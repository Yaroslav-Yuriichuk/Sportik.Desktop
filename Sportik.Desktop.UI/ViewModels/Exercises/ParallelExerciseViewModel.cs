using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Common.Timers;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Automation;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Core.Models.Sound;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Core.States.Exercises;
using Sportik.Desktop.UI.Helpers;

namespace Sportik.Desktop.UI.ViewModels.Exercises
{
    internal sealed class ParallelExerciseViewModel : ViewModel, IDisposable
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (SetField(ref _isEnabled, value))
                {
                    _updateCts?.Cancel();
                    _updateCts = new CancellationTokenSource();

                    _ = UpdateExerciseSettingsAsync(_updateCts.Token);
                }
            }
        }

        private TimeSpan _reminderTime;

        public TimeSpan ReminderTime
        {
            get => _reminderTime;
            set => SetField(ref _reminderTime, value);
        }

        private TimeSpan _executionTime;

        public TimeSpan ExecutionTime
        {
            get => _executionTime;
            set => SetField(ref _executionTime, value);
        }

        private ParallelExerciseState _state;

        public ParallelExerciseState State
        {
            get => _state;
            set => SetField(ref _state, value);
        }

        public IReactiveCommand CompleteCommand { get; }

        public ICommand ExecuteCommand { get; }

        private IExercisesService ExercisesService => App.ServiceProvider.GetService<IExercisesService>();
        private IExerciseSettingsService ExerciseSettingsService => App.ServiceProvider.GetService<IExerciseSettingsService>();
        private IExerciseStatisticsService ExerciseStatisticsService => App.ServiceProvider.GetService<IExerciseStatisticsService>();
        private IExerciseTimersService ExerciseTimersService => App.ServiceProvider.GetService<IExerciseTimersService>();
        private IEventsService EventsService => App.ServiceProvider.GetService<IEventsService>();
        private IReminderService ReminderService => App.ServiceProvider.GetService<IReminderService>();
        private ISoundService SoundService => App.ServiceProvider.GetService<ISoundService>();

        private readonly Guid _exerciseId;

        private CancellationTokenSource _updateCts = new CancellationTokenSource();
        private CancellationTokenSource _completeCts = new CancellationTokenSource();

        private readonly ITimer _exerciseReminderUpdateTimer;
        private readonly ITimer _exerciseExecutionUpdateTimer;

        public ParallelExerciseViewModel(Exercise exercise)
        {
            _exerciseId = exercise.Id;

            Name = exercise.Name;
            SetField(ref _isEnabled, exercise.Settings.IsEnabled, nameof(IsEnabled));

            CompleteCommand = new ReactiveRelayCommand(CompleteExercise, false);
            ExecuteCommand = new ReactiveRelayCommand(ExecuteExercise);

            _exerciseReminderUpdateTimer = new DefaultTimerBuilder()
                .SetInterval(TimeSpan.FromSeconds(0.5))
                .SetCallback(UpdateReminderTime)
                .SetLoop()
                .Build();

            _exerciseExecutionUpdateTimer = new DefaultTimerBuilder()
                .SetInterval(TimeSpan.FromSeconds(0.5))
                .SetCallback(UpdateExecutionTime)
                .SetLoop()
                .Build();

            ParallelExerciseState state = ReminderService.GetExerciseState<ParallelExerciseState>(_exerciseId);

            switch (state)
            {
                case ParallelExerciseState.Unknown:
                case ParallelExerciseState.Disabled:
                    break;
                case ParallelExerciseState.WaitingBeforeForceExecution:
                case ParallelExerciseState.WaitingWithForceExecution:
                    _exerciseReminderUpdateTimer.Start();
                    break;
                case ParallelExerciseState.Executing:
                    CompleteCommand.IsExecutable = true;
                    _exerciseExecutionUpdateTimer.Start();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            State = state;

            ITimer timer = ExerciseTimersService.GetTimer(_exerciseId, ReminderMode.Parallel);

            if (timer != null)
            {
                ReminderTime = timer.Interval - timer.ElapsedTime;
                ExecutionTime = timer.Interval - timer.ElapsedTime;
            }

            EventsService.AddListener<ParallelExerciseStateChangedEventArgs>(EventsService_Event);
        }

        public void Dispose()
        {
            _updateCts?.Cancel();
            _completeCts?.Cancel();

            _exerciseReminderUpdateTimer.Stop();
            _exerciseExecutionUpdateTimer.Stop();

            EventsService.RemoveListener<ParallelExerciseStateChangedEventArgs>(EventsService_Event);
        }

        private async Task UpdateExerciseSettingsAsync(CancellationToken cancellationToken)
        {
            ExerciseSettingsDelta exerciseSettingsDelta = new ExerciseSettingsDelta
            {
                Change = ExerciseSettingsChange.IsEnabled,
                IsEnabled = IsEnabled,
            };

            UpdateExerciseSettingsModel updateModel = new UpdateExerciseSettingsModel(
                _exerciseId,
                exerciseSettingsDelta);

            await ExerciseSettingsService.UpdateAsync(updateModel, cancellationToken);
        }

        private void EventsService_Event(ParallelExerciseStateChangedEventArgs args)
        {
            if (args.ExerciseId != _exerciseId)
            {
                return;
            }

            _ = UIThreadHelper.RunOnUIThreadAsync(() =>
            {
                ParallelExerciseState previousState = args.PreviousState;

                switch (previousState)
                {
                    case ParallelExerciseState.Unknown:
                    case ParallelExerciseState.Disabled:
                        break;
                    case ParallelExerciseState.WaitingBeforeForceExecution:
                    case ParallelExerciseState.WaitingWithForceExecution:
                        _exerciseReminderUpdateTimer.Stop();
                        break;
                    case ParallelExerciseState.Executing:
                        CompleteCommand.IsExecutable = false;
                        _exerciseExecutionUpdateTimer.Stop();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                ParallelExerciseState currentState = args.CurrentState;

                switch (currentState)
                {
                    case ParallelExerciseState.Unknown:
                    case ParallelExerciseState.Disabled:
                        break;
                    case ParallelExerciseState.WaitingBeforeForceExecution:
                    case ParallelExerciseState.WaitingWithForceExecution:
                        _exerciseReminderUpdateTimer.Start();
                        break;
                    case ParallelExerciseState.Executing:
                        CompleteCommand.IsExecutable = true;
                        _exerciseExecutionUpdateTimer.Start();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                State = currentState;

                ITimer timer = ExerciseTimersService.GetTimer(_exerciseId, ReminderMode.Parallel);

                if (timer != null)
                {
                    ReminderTime = timer.Interval - timer.ElapsedTime;
                    ExecutionTime = timer.Interval - timer.ElapsedTime;
                }
            });
        }

        private void UpdateReminderTime(object sender, EventArgs e)
        {
            _ = UIThreadHelper.RunOnUIThreadAsync(() =>
            {
                ITimer timer = ExerciseTimersService.GetTimer(_exerciseId, ReminderMode.Parallel);

                if (timer != null)
                {
                    ReminderTime = timer.Interval - timer.ElapsedTime;
                }
            });
        }

        private void UpdateExecutionTime(object sender, EventArgs e)
        {
            _ = UIThreadHelper.RunOnUIThreadAsync(() =>
            {
                ITimer timer = ExerciseTimersService.GetTimer(_exerciseId, ReminderMode.Parallel);

                if (timer != null)
                {
                    ExecutionTime = timer.Interval - timer.ElapsedTime;
                }
            });
        }

        private void CompleteExercise()
        {
            _completeCts?.Cancel();
            _completeCts = new CancellationTokenSource();

            _ = CompleteExerciseAsync(_completeCts.Token);
        }

        private void ExecuteExercise()
        {
            EventsService.RaiseEvent(new ExerciseForceExecutionRequestedEventArgs(_exerciseId));
        }

        private async Task CompleteExerciseAsync(CancellationToken cancellationToken)
        {
            CompleteCommand.IsExecutable = false;

            OperationResult<Exercise> result = await ExercisesService.GetByIdAsync(_exerciseId, cancellationToken);

            if (!result.Succeeded)
            {
                // TODO: Handle error.
                return;
            }

            Exercise exercise = result.Value;

            AddExerciseSetModel addModel = new AddExerciseSetModel(null, exercise.Settings.TargetRepetitions, DateTimeOffset.UtcNow);
            await ExerciseStatisticsService.AddSetAsync(addModel, exercise.Id, cancellationToken);

            EventsService.RaiseEvent(new ExerciseCompleteRequestedEventArgs(_exerciseId));

            SoundSource soundSource = SoundSource.Custom("Assets/Sound/Completed.mp3");
            await SoundService.PlayAsync(soundSource, cancellationToken);
        }
    }
}
