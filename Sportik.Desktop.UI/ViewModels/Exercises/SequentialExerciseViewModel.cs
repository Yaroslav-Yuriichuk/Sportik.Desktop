using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Backend.Domain.Common;
using Sportik.Desktop.Core.Common.Timers;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Automation;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Core.Models.Sound;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Core.States.Exercises;
using Sportik.Desktop.UI.Helpers;

namespace Sportik.Desktop.UI.ViewModels.Exercises
{
    internal sealed class SequentialExerciseViewModel : ViewModel, IDisposable
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

        private SequentialExerciseState _state;

        public SequentialExerciseState State
        {
            get => _state;
            set => SetField(ref _state, value);
        }

        public ICommand CompleteCommand { get; }

        public ICommand ExecuteCommand { get; }

        public ICommand SwitchCommand { get; }

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

        public SequentialExerciseViewModel(Exercise exercise)
        {
            _exerciseId = exercise.Id;

            Name = exercise.Name;
            SetField(ref _isEnabled, exercise.Settings.IsEnabled);

            CompleteCommand = new ReactiveRelayCommand(CompleteExercise);
            ExecuteCommand = new ReactiveRelayCommand(ExecuteExercise);
            SwitchCommand = new ReactiveRelayCommand(SwitchExercise);

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

            SequentialExerciseState state = ReminderService.GetExerciseState<SequentialExerciseState>(_exerciseId);

            switch (state)
            {
                case SequentialExerciseState.Unknown:
                case SequentialExerciseState.Disabled:
                case SequentialExerciseState.Queued:
                    break;
                case SequentialExerciseState.WaitingBeforeForceExecution:
                case SequentialExerciseState.WaitingWithForceExecution:
                    _exerciseReminderUpdateTimer.Start();
                    break;
                case SequentialExerciseState.Executing:
                    _exerciseExecutionUpdateTimer.Start();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            State = state;

            ITimer timer = ExerciseTimersService.GetTimer(_exerciseId, ReminderMode.Sequential);

            if (timer != null)
            {
                ReminderTime = timer.Interval - timer.ElapsedTime;
                ExecutionTime = timer.Interval - timer.ElapsedTime;
            }

            EventsService.AddListener<SequentialExerciseStateChangedEventArgs>(EventsService_Event);
        }

        public void Dispose()
        {
            _updateCts?.Cancel();

            _exerciseReminderUpdateTimer.Stop();
            _exerciseExecutionUpdateTimer.Stop();

            EventsService.RemoveListener<SequentialExerciseStateChangedEventArgs>(EventsService_Event);
        }

        private async Task UpdateExerciseSettingsAsync(CancellationToken cancellationToken)
        {
            ExerciseSettingsDelta exerciseSettingsDelta = new ExerciseSettingsDelta
            {
                Change = ExerciseSettingsChange.IsEnabled,
                IsEnabled = IsEnabled,
            };

            await ExerciseSettingsService.UpdateAsync(exerciseSettingsDelta, _exerciseId, cancellationToken);
        }

        private void EventsService_Event(SequentialExerciseStateChangedEventArgs args)
        {
            if (args.ExerciseId != _exerciseId)
            {
                return;
            }

            _ = UIThreadHelper.RunOnUIThreadAsync(() =>
            {
                SequentialExerciseState previousState = args.PreviousState;

                switch (previousState)
                {
                    case SequentialExerciseState.Unknown:
                    case SequentialExerciseState.Disabled:
                    case SequentialExerciseState.Queued:
                        break;
                    case SequentialExerciseState.WaitingBeforeForceExecution:
                    case SequentialExerciseState.WaitingWithForceExecution:
                        _exerciseReminderUpdateTimer.Stop();
                        break;
                    case SequentialExerciseState.Executing:
                        _exerciseExecutionUpdateTimer.Stop();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                SequentialExerciseState currentState = args.CurrentState;

                switch (currentState)
                {
                    case SequentialExerciseState.Unknown:
                    case SequentialExerciseState.Disabled:
                    case SequentialExerciseState.Queued:
                        break;
                    case SequentialExerciseState.WaitingBeforeForceExecution:
                    case SequentialExerciseState.WaitingWithForceExecution:
                        _exerciseReminderUpdateTimer.Start();
                        break;
                    case SequentialExerciseState.Executing:
                        _exerciseExecutionUpdateTimer.Start();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                State = currentState;

                ITimer timer = ExerciseTimersService.GetTimer(_exerciseId, ReminderMode.Sequential);

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
                ITimer timer = ExerciseTimersService.GetTimer(_exerciseId, ReminderMode.Sequential);

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
                ITimer timer = ExerciseTimersService.GetTimer(_exerciseId, ReminderMode.Sequential);

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

        private void SwitchExercise()
        {
            EventsService.RaiseEvent(new ExerciseSwitchRequestedEventArgs(_exerciseId));
        }

        private async Task CompleteExerciseAsync(CancellationToken cancellationToken)
        {
            OperationResult<Exercise> result = await ExercisesService.GetByIdAsync(_exerciseId, cancellationToken);

            if (!result.Succeeded)
            {
                // TODO: Handle error.
                return;
            }

            ExerciseStatisticsDelta exerciseStatisticsDelta = new ExerciseStatisticsDelta
            {
                Exercise = null,
                Sets = 1,
                Repetitions = result.Value.Settings.TargetRepetitions,
            };

            await ExerciseStatisticsService.AddExerciseStatisticsDeltaAsync(exerciseStatisticsDelta, DateTime.Today, cancellationToken);

            EventsService.RaiseEvent(new ExerciseCompleteRequestedEventArgs(_exerciseId));

            SoundSource soundSource = SoundSource.Custom("Assets/Sound/Completed.mp3");
            await SoundService.PlayAsync(soundSource, cancellationToken);
        }
    }
}
