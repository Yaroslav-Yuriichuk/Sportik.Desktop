using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Core;
using Sportik.Helpers;
using Sportik.Models;
using Sportik.Models.Settings;
using Sportik.Models.Statistics;
using Sportik.Services.Events;
using Sportik.Services.Reminders;
using Sportik.Services.Settings;
using Sportik.Services.Statistics;

namespace Sportik.ViewModels.Exercises
{
    internal sealed class ExerciseViewModel : ViewModel, IDisposable
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

        private bool _isInWaitingState;

        public bool IsInWaitingState
        {
            get => _isInWaitingState;
            set => SetField(ref _isInWaitingState, value);
        }

        private bool _isInExecutingState;

        public bool IsInExecutingState
        {
            get => _isInExecutingState;
            set => SetField(ref _isInExecutingState, value);
        }

        public ICommand CompleteCommand { get; private set; }

        private IExerciseSettingsService ExerciseSettingsService => App.ServiceProvider.GetService<IExerciseSettingsService>();
        private IExerciseStatisticsService ExerciseStatisticsService => App.ServiceProvider.GetService<IExerciseStatisticsService>();
        private IExerciseTimersService ExerciseTimersService => App.ServiceProvider.GetService<IExerciseTimersService>();
        private IEventsService EventsService => App.ServiceProvider.GetService<IEventsService>();
        private IReminderService ReminderService => App.ServiceProvider.GetService<IReminderService>();

        private readonly Exercise _exercise;

        private CancellationTokenSource _updateCts = new CancellationTokenSource();
        private CancellationTokenSource _completeCts = new CancellationTokenSource();

        private readonly ITimer _exerciseReminderUpdateTimer;
        private readonly ITimer _exerciseExecutionUpdateTimer;

        public ExerciseViewModel(Exercise exercise)
        {
            _exercise = exercise;

            Name = exercise.Name;
            SetField(ref _isEnabled, exercise.ExerciseSettings.IsEnabled);

            CompleteCommand = new RelayCommand<object>(CompleteExercise);

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

            ExerciseStateKind state = ReminderService.GetExerciseState(exercise);

            switch (state)
            {
                case ExerciseStateKind.Unknown:
                case ExerciseStateKind.Disabled:
                    IsInWaitingState = false;
                    IsInExecutingState = false;
                    break;
                case ExerciseStateKind.Waiting:
                    IsInWaitingState = true;
                    IsInExecutingState = false;
                    _exerciseReminderUpdateTimer.Start();
                    break;
                case ExerciseStateKind.Executing:
                    IsInWaitingState = false;
                    IsInExecutingState = true;
                    _exerciseExecutionUpdateTimer.Start();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ITimer timer = ExerciseTimersService.GetTimer(_exercise);

            if (timer != null)
            {
                ReminderTime = timer.Interval - timer.ElapsedTime;
                ExecutionTime = timer.Interval - timer.ElapsedTime;
            }

            EventsService.Event += EventsService_Event;
        }

        public void Dispose()
        {
            _updateCts?.Cancel();

            _exerciseReminderUpdateTimer.Stop();
            _exerciseExecutionUpdateTimer.Stop();

            EventsService.Event -= EventsService_Event;
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

        private void EventsService_Event(EventArgs args)
        {
            if (args is ExerciseStateChangedEventArgs stateChangedEventArgs && CompareHelper.EqualById(_exercise, stateChangedEventArgs.Exercise))
            {
                _ = UIThreadHelper.RunOnUIThreadAsync(() =>
                {
                    ExerciseStateKind previousState = stateChangedEventArgs.PreviousState;

                    switch (previousState)
                    {
                        case ExerciseStateKind.Unknown:
                        case ExerciseStateKind.Disabled:
                            break;
                        case ExerciseStateKind.Waiting:
                            _exerciseReminderUpdateTimer.Stop();
                            IsInWaitingState = false;
                            break;
                        case ExerciseStateKind.Executing:
                            _exerciseExecutionUpdateTimer.Stop();
                            IsInExecutingState = false;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    ExerciseStateKind currentState = stateChangedEventArgs.CurrentState;

                    switch (currentState)
                    {
                        case ExerciseStateKind.Unknown:
                        case ExerciseStateKind.Disabled:
                            break;
                        case ExerciseStateKind.Waiting:
                            IsInWaitingState = true;
                            _exerciseReminderUpdateTimer.Start();
                            break;
                        case ExerciseStateKind.Executing:
                            IsInExecutingState = true;
                            _exerciseExecutionUpdateTimer.Start();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    ITimer timer = ExerciseTimersService.GetTimer(_exercise);

                    if (timer != null)
                    {
                        ReminderTime = timer.Interval - timer.ElapsedTime;
                        ExecutionTime = timer.Interval - timer.ElapsedTime;
                    }
                });
            }
        }

        private void UpdateReminderTime(object sender, EventArgs e)
        {
            _ = UIThreadHelper.RunOnUIThreadAsync(() =>
            {
                ITimer timer = ExerciseTimersService.GetTimer(_exercise);

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
                ITimer timer = ExerciseTimersService.GetTimer(_exercise);

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

        private async Task CompleteExerciseAsync(CancellationToken cancellationToken)
        {
            ExerciseSettings exerciseSettings = await ExerciseSettingsService.GetExerciseSettingsAsync(_exercise, cancellationToken);

            ExerciseStatisticsDelta exerciseStatisticsDelta = new ExerciseStatisticsDelta()
            {
                Exercise = _exercise,
                Sets = 1,
                Repetitions = exerciseSettings.TargetRepetitions,
            };

            await ExerciseStatisticsService.AddExerciseStatisticsDeltaAsync(exerciseStatisticsDelta, DateTime.Today, cancellationToken);
        }
    }
}
