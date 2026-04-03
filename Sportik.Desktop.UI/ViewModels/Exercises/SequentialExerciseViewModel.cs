using System;
using System.Threading;
using System.Threading.Tasks;
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

        private TimeSpan _snoozingTime;

        public TimeSpan SnoozingTime
        {
            get => _snoozingTime;
            set => SetField(ref _snoozingTime, value);
        }

        private SequentialExerciseState _state;

        public SequentialExerciseState State
        {
            get => _state;
            set => SetField(ref _state, value);
        }

        public IReactiveCommand CompleteCommand { get; }

        public IReactiveCommand ExecuteCommand { get; }

        public IReactiveCommand SwitchCommand { get; }

        public Guid ExerciseId { get; }

        private IExercisesService ExercisesService => App.ServiceProvider.GetService<IExercisesService>();
        private IExerciseSettingsService ExerciseSettingsService => App.ServiceProvider.GetService<IExerciseSettingsService>();
        private IExerciseStatisticsService ExerciseStatisticsService => App.ServiceProvider.GetService<IExerciseStatisticsService>();
        private IExerciseTimersService ExerciseTimersService => App.ServiceProvider.GetService<IExerciseTimersService>();
        private IEventsService EventsService => App.ServiceProvider.GetService<IEventsService>();
        private IReminderService ReminderService => App.ServiceProvider.GetService<IReminderService>();
        private ISoundService SoundService => App.ServiceProvider.GetService<ISoundService>();

        private CancellationTokenSource _updateCts = new CancellationTokenSource();
        private CancellationTokenSource _completeCts = new CancellationTokenSource();

        private readonly ITimer _exerciseReminderUpdateTimer;
        private readonly ITimer _exerciseExecutionUpdateTimer;
        private readonly ITimer _exerciseSnoozingUpdateTimer;

        public SequentialExerciseViewModel(Exercise exercise)
        {
            ExerciseId = exercise.Id;

            Name = exercise.Name;
            SetField(ref _isEnabled, exercise.Settings.IsEnabled, nameof(IsEnabled));

            CompleteCommand = new ReactiveRelayCommand(CompleteExercise, false);
            ExecuteCommand = new ReactiveRelayCommand(ExecuteExercise, false);
            SwitchCommand = new ReactiveRelayCommand(SwitchExercise, false);

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

            _exerciseSnoozingUpdateTimer = new DefaultTimerBuilder()
                .SetInterval(TimeSpan.FromSeconds(0.5))
                .SetCallback(UpdateSnoozingTime)
                .SetLoop()
                .Build();

            SequentialExerciseState state = ReminderService.GetExerciseState<SequentialExerciseState>(ExerciseId);

            switch (state)
            {
                case SequentialExerciseState.Unknown:
                case SequentialExerciseState.Disabled:
                    break;
                case SequentialExerciseState.Queued:
                    SwitchCommand.IsExecutable = true;
                    break;
                case SequentialExerciseState.WaitingBeforeForceExecution:
                    _exerciseReminderUpdateTimer.Start();
                    break;
                case SequentialExerciseState.WaitingWithForceExecution:
                    ExecuteCommand.IsExecutable = true;
                    _exerciseReminderUpdateTimer.Start();
                    break;
                case SequentialExerciseState.Executing:
                    CompleteCommand.IsExecutable = true;
                    _exerciseExecutionUpdateTimer.Start();
                    break;
                case SequentialExerciseState.Snoozed:
                    ExecuteCommand.IsExecutable = true;
                    _exerciseSnoozingUpdateTimer.Start();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            State = state;

            ITimer timer = ExerciseTimersService.GetTimer(ExerciseId, ReminderMode.Sequential);

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
            _completeCts?.Cancel();

            _exerciseReminderUpdateTimer.Stop();
            _exerciseExecutionUpdateTimer.Stop();
            _exerciseSnoozingUpdateTimer.Stop();

            EventsService.RemoveListener<SequentialExerciseStateChangedEventArgs>(EventsService_Event);
        }

        private async Task UpdateExerciseSettingsAsync(CancellationToken cancellationToken)
        {
            ExerciseSettingsDelta exerciseSettingsDelta = new ExerciseSettingsDelta
            {
                Change = ExerciseSettingsChange.IsEnabled,
                IsEnabled = IsEnabled,
            };

            UpdateExerciseSettingsModel updateModel = new UpdateExerciseSettingsModel(
                ExerciseId,
                exerciseSettingsDelta);

            await ExerciseSettingsService.UpdateAsync(updateModel, cancellationToken);
        }

        private void EventsService_Event(SequentialExerciseStateChangedEventArgs args)
        {
            if (args.ExerciseId != ExerciseId)
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
                        break;
                    case SequentialExerciseState.Queued:
                        SwitchCommand.IsExecutable = false;
                        break;
                    case SequentialExerciseState.WaitingBeforeForceExecution:
                        _exerciseReminderUpdateTimer.Stop();
                        break;
                    case SequentialExerciseState.WaitingWithForceExecution:
                        ExecuteCommand.IsExecutable = false;
                        _exerciseReminderUpdateTimer.Stop();
                        break;
                    case SequentialExerciseState.Executing:
                        CompleteCommand.IsExecutable = false;
                        _exerciseExecutionUpdateTimer.Stop();
                        break;
                    case SequentialExerciseState.Snoozed:
                        ExecuteCommand.IsExecutable = false;
                        _exerciseSnoozingUpdateTimer.Stop();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                SequentialExerciseState currentState = args.CurrentState;

                switch (currentState)
                {
                    case SequentialExerciseState.Unknown:
                    case SequentialExerciseState.Disabled:
                        break;
                    case SequentialExerciseState.Queued:
                        SwitchCommand.IsExecutable = true;
                        break;
                    case SequentialExerciseState.WaitingBeforeForceExecution:
                        _exerciseReminderUpdateTimer.Start();
                        break;
                    case SequentialExerciseState.WaitingWithForceExecution:
                        ExecuteCommand.IsExecutable = true;
                        _exerciseReminderUpdateTimer.Start();
                        break;
                    case SequentialExerciseState.Executing:
                        CompleteCommand.IsExecutable = true;
                        _exerciseExecutionUpdateTimer.Start();
                        break;
                    case SequentialExerciseState.Snoozed:
                        ExecuteCommand.IsExecutable = true;
                        _exerciseSnoozingUpdateTimer.Start();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                State = currentState;

                ITimer timer = ExerciseTimersService.GetTimer(ExerciseId, ReminderMode.Sequential);

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
                ITimer timer = ExerciseTimersService.GetTimer(ExerciseId, ReminderMode.Sequential);

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
                ITimer timer = ExerciseTimersService.GetTimer(ExerciseId, ReminderMode.Sequential);

                if (timer != null)
                {
                    ExecutionTime = timer.Interval - timer.ElapsedTime;
                }
            });
        }

        private void UpdateSnoozingTime(object sender, EventArgs e)
        {
            _ = UIThreadHelper.RunOnUIThreadAsync(() =>
            {
                ITimer timer = ExerciseTimersService.GetTimer(ExerciseId, ReminderMode.Sequential);

                if (timer != null)
                {
                    SnoozingTime = timer.Interval - timer.ElapsedTime;
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
            EventsService.RaiseEvent(new ExerciseForceExecutionRequestedEventArgs(ExerciseId));
        }

        private void SwitchExercise()
        {
            EventsService.RaiseEvent(new ExerciseSwitchRequestedEventArgs(ExerciseId));
        }

        private async Task CompleteExerciseAsync(CancellationToken cancellationToken)
        {
            OperationResult<Exercise> result = await ExercisesService.GetByIdAsync(ExerciseId, cancellationToken);

            if (!result.Succeeded)
            {
                // TODO: Handle error.
                return;
            }

            Exercise exercise = result.Value;

            AddExerciseSetModel addModel = new AddExerciseSetModel(null, exercise.Settings.TargetRepetitions, DateTimeOffset.UtcNow, exercise.Id);
            await ExerciseStatisticsService.AddSetAsync(addModel, cancellationToken);

            EventsService.RaiseEvent(new ExerciseCompleteRequestedEventArgs(ExerciseId));

            SoundSource soundSource = SoundSource.Custom("Assets/Sound/Completed.mp3");
            await SoundService.PlayAsync(soundSource, cancellationToken);
        }
    }
}
