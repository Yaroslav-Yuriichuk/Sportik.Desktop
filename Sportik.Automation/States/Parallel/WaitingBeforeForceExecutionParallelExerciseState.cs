using Sportik.Automation.Models;
using Sportik.Automation.Services;
using Sportik.Core.Events;
using Sportik.Core.Helpers;
using Sportik.Core.Models.Settings;
using Sportik.Core.Services.Interfaces;
using Sportik.Core.Timers;
using Sportik.Notification.Models;
using Sportik.Notification.Services;
using System;
using Sportik.Automation.Constants;

namespace Sportik.Automation.States.Parallel
{
    internal sealed class WaitingBeforeForceExecutionParallelExerciseState : ParallelExerciseState
    {
        private readonly IEventsService _eventsService;
        private readonly IExerciseTimersService _exerciseTimersService;
        private readonly Func<IExerciseSettingsService> _exerciseSettingsServiceFactory;
        private readonly Func<INotificationService> _notificationServiceFactory;

        public override States.ParallelExerciseState ExerciseState => States.ParallelExerciseState.WaitingBeforeForceExecution;

        private ITimer _forceExecutionTimer;

        public WaitingBeforeForceExecutionParallelExerciseState(ParallelExerciseStatesContext context, IEventsService eventsService, IExerciseTimersService exerciseTimersService,
            Func<IExerciseSettingsService> exerciseSettingsServiceFactory, Func<INotificationService> notificationServiceFactory) : base(context)
        {
            _eventsService = eventsService;
            _exerciseTimersService = exerciseTimersService;
            _exerciseSettingsServiceFactory = exerciseSettingsServiceFactory;
            _notificationServiceFactory = notificationServiceFactory;
        }

        public override void Enter()
        {
            IExerciseSettingsService exerciseSettingsService = _exerciseSettingsServiceFactory();

            _eventsService.AddListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ExerciseTimeBetweenSetsChangedEventArgs>(EventsService_Event);

            ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise, ReminderMode.Parallel);

            timer.Loop = false;
            timer.Interval = exerciseSettingsService.GetExerciseSettings(Context.Exercise).TimeBetweenSets;

            timer.Elapsed += Timer_Elapsed;

            if (timer.IsPaused)
            {
                timer.Resume();
            }
            else
            {
                timer.Start();
            }

            _forceExecutionTimer = new DefaultTimerBuilder()
                .SetInterval(AutomationConstants.TimeBeforeForceExecution)
                .SetCallback(ForceExecutionTimer_Elapsed)
                .Build();

            _forceExecutionTimer.Start();
        }

        public override void Exit()
        {
            _eventsService.RemoveListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<ExerciseTimeBetweenSetsChangedEventArgs>(EventsService_Event);

            ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise, ReminderMode.Parallel);

            timer.Elapsed -= Timer_Elapsed;

            if (timer.IsRunning)
            {
                timer.Pause();
            }

            _forceExecutionTimer.Stop();
        }

        private void EventsService_Event(ExerciseIsEnabledChangedEventArgs args)
        {
            if (CompareHelper.EqualById(Context.Exercise, args.Exercise) && !args.IsEnabled)
            {
                Context.Switch(Context.DisabledExerciseState);
            }
        }

        private void EventsService_Event(ExerciseTimeBetweenSetsChangedEventArgs args)
        {
            if (CompareHelper.EqualById(Context.Exercise, args.Exercise))
            {
                ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise, ReminderMode.Parallel);
                timer.Interval = args.TimeBetweenSets;
            }
        }

        private void Timer_Elapsed(object sender, EventArgs args)
        {
            IExerciseSettingsService exerciseSettingsService = _exerciseSettingsServiceFactory();
            INotificationService notificationService = _notificationServiceFactory();

            ExerciseSettings exerciseSettings = exerciseSettingsService.GetExerciseSettings(Context.Exercise);

            notificationService.ShowReminder(Context.Exercise, new ReminderNotification()
            {
                Title = $"{Context.Exercise.Name} reminder!",
                Texts = new[]
                {
                    $"You have {exerciseSettings.ExecutionTime.TotalMinutes} minutes to complete {Context.Exercise.Name.ToLower()} exercise.",
                    $"Target repetitions: {exerciseSettings.TargetRepetitions}.",
                },
                ExpirationTime = exerciseSettings.ExecutionTime,
            });

            Context.Switch(Context.ExecutingExerciseState);
        }

        private void ForceExecutionTimer_Elapsed(object sender, EventArgs e)
        {
            Context.Switch(Context.WaitingWithForceExecutionExerciseState);
        }
    }
}
