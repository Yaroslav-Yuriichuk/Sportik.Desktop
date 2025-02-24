using System;
using Sportik.Automation.Events;
using Sportik.Automation.Models;
using Sportik.Automation.Services;
using Sportik.Automation.States;
using Sportik.Core.Helpers;
using Sportik.Core.Models.Settings;
using Sportik.Core.Models.Statistics;
using Sportik.Core.Models;
using Sportik.Core.Services.Interfaces;
using Sportik.Core.Timers;
using Sportik.UWP.Helpers;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Sound.Models;
using Sportik.Sound.Services.Interfaces;

namespace Sportik.UWP.ViewModels.Exercises
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

        private IExerciseSettingsService ExerciseSettingsService => App.ServiceProvider.GetService<IExerciseSettingsService>();
        private IExerciseStatisticsService ExerciseStatisticsService => App.ServiceProvider.GetService<IExerciseStatisticsService>();
        private IExerciseTimersService ExerciseTimersService => App.ServiceProvider.GetService<IExerciseTimersService>();
        private IEventsService EventsService => App.ServiceProvider.GetService<IEventsService>();
        private IReminderService ReminderService => App.ServiceProvider.GetService<IReminderService>();
        private ISoundService SoundService => App.ServiceProvider.GetService<ISoundService>();

        private readonly Exercise _exercise;

        private CancellationTokenSource _updateCts = new CancellationTokenSource();
        private CancellationTokenSource _completeCts = new CancellationTokenSource();

        private readonly ITimer _exerciseReminderUpdateTimer;
        private readonly ITimer _exerciseExecutionUpdateTimer;

        public SequentialExerciseViewModel(Exercise exercise)
        {
            _exercise = exercise;

            Name = exercise.Name;
            SetField(ref _isEnabled, exercise.ExerciseSettings.IsEnabled);

            CompleteCommand = new RelayCommand<object>(CompleteExercise);
            ExecuteCommand = new RelayCommand<object>(ExecuteExercise);

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

            SequentialExerciseState state = ReminderService.GetExerciseState<SequentialExerciseState>(exercise);

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

            ITimer timer = ExerciseTimersService.GetTimer(_exercise, ReminderMode.Sequential);

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
            ExerciseSettingsDelta exerciseSettingsDelta = new ExerciseSettingsDelta()
            {
                Change = ExerciseSettingsChange.IsEnabled,
                IsEnabled = IsEnabled,
            };

            await ExerciseSettingsService.UpdateExerciseSettingsAsync(exerciseSettingsDelta, _exercise, cancellationToken);
        }

        private void EventsService_Event(SequentialExerciseStateChangedEventArgs args)
        {
            if (!CompareHelper.EqualById(_exercise, args.Exercise))
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

                ITimer timer = ExerciseTimersService.GetTimer(_exercise, ReminderMode.Sequential);

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
                ITimer timer = ExerciseTimersService.GetTimer(_exercise, ReminderMode.Sequential);

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
                ITimer timer = ExerciseTimersService.GetTimer(_exercise, ReminderMode.Sequential);

                if (timer != null)
                {
                    ExecutionTime = timer.Interval - timer.ElapsedTime;
                }
            });
        }

        private void CompleteExercise(object parameter)
        {
            _completeCts?.Cancel();
            _completeCts = new CancellationTokenSource();

            _ = CompleteExerciseAsync(_completeCts.Token);
        }

        private void ExecuteExercise(object parameter)
        {
            EventsService.RaiseEvent(new ExerciseForceExecutionRequestedEventArgs(_exercise));
        }

        private async Task CompleteExerciseAsync(CancellationToken cancellationToken)
        {
            ExerciseSettings exerciseSettings = await ExerciseSettingsService.GetExerciseSettingsAsync(_exercise, cancellationToken);

            ExerciseStatisticsDelta exerciseStatisticsDelta = new ExerciseStatisticsDelta
            {
                Exercise = _exercise,
                Sets = 1,
                Repetitions = exerciseSettings.TargetRepetitions,
            };

            await ExerciseStatisticsService.AddExerciseStatisticsDeltaAsync(exerciseStatisticsDelta, DateTime.Today, cancellationToken);

            EventsService.RaiseEvent(new ExerciseCompleteRequestedEventArgs(_exercise));

            SoundSource soundSource = SoundSource.Custom("Assets/Sound/Completed.mp3");
            await SoundService.PlayAsync(soundSource, cancellationToken);
        }
    }
}
