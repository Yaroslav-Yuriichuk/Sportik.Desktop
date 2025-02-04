using System;
using Sportik.Automation.Services;
using Sportik.Automation.States;
using Sportik.Core.Events;
using Sportik.Core.Helpers;
using Sportik.Core.Models.Settings;
using Sportik.Core.Services.Interfaces;
using Sportik.Notification.Models;
using Sportik.Notification.Services;
using Sportik.UWP.Core;

namespace Sportik.UWP.Services.Reminders.States
{
    internal sealed class WaitingExerciseState : ExerciseState
    {
        private readonly IEventsService _eventsService;
        private readonly IExerciseTimersService _exerciseTimersService;
        private readonly IExerciseSettingsService _exerciseSettingsService;
        private readonly INotificationService _notificationService;

        public override ExerciseStateKind Kind => ExerciseStateKind.Waiting;

        public WaitingExerciseState(ExerciseStatesContext context, IEventsService eventsService, IExerciseTimersService exerciseTimersService,
            IExerciseSettingsService exerciseSettingsService, INotificationService notificationService) : base(context)
        {
            _eventsService = eventsService;
            _exerciseTimersService = exerciseTimersService;
            _exerciseSettingsService = exerciseSettingsService;
            _notificationService = notificationService;
        }

        public override void Enter()
        {
            _eventsService.Event += EventsService_Event;

            ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise);

            timer.Loop = false;
            timer.Interval = _exerciseSettingsService.GetExerciseSettings(Context.Exercise).TimeBetweenSets;

            timer.Elapsed += Timer_Elapsed;

            if (timer.IsPaused)
            {
                timer.Resume();
            }
            else
            {
                timer.Start();
            }
        }

        public override void Exit()
        {
            _eventsService.Event -= EventsService_Event;

            ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise);

            timer.Elapsed -= Timer_Elapsed;

            if (timer.IsRunning)
            {
                timer.Pause();
            }
        }

        private void EventsService_Event(EventArgs args)
        {
            if (args is ExerciseIsEnabledChangedEventArgs isEnabledChangedEventArgs)
            {
                if (CompareHelper.EqualById(Context.Exercise, isEnabledChangedEventArgs.Exercise) && !isEnabledChangedEventArgs.IsEnabled)
                {
                    Context.Switch(Context.DisabledExerciseState);
                }
            }

            if (args is ExerciseTimeBetweenSetsChangedEventArgs timeBetweenSetsChangedEventArgs)
            {
                if (CompareHelper.EqualById(Context.Exercise, timeBetweenSetsChangedEventArgs.Exercise))
                {
                    ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise);
                    timer.Interval = timeBetweenSetsChangedEventArgs.TimeBetweenSets;
                }
            }
        }

        private void Timer_Elapsed(object sender, EventArgs e)
        {
            ExerciseSettings exerciseSettings = _exerciseSettingsService.GetExerciseSettings(Context.Exercise);

            _notificationService.ShowReminder(new ReminderNotification()
            {
                Title = $"{Context.Exercise.Name} reminder!",
                Texts = new []
                {
                    $"You have {exerciseSettings.ExecutionTime.TotalMinutes} minutes to complete {Context.Exercise.Name.ToLower()} exercise.",
                    $"Target repetitions: {exerciseSettings.TargetRepetitions}.",
                },
                ExpirationTime = exerciseSettings.ExecutionTime,
            });

            Context.Switch(Context.ExecutingExerciseState);
        }
    }
}
