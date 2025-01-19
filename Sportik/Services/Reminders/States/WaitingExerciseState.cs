using System;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Core;
using Sportik.Helpers;
using Sportik.Models.Notifications;
using Sportik.Models.Settings;
using Sportik.Services.Events;
using Sportik.Services.Notifications;
using Sportik.Services.Settings;

namespace Sportik.Services.Reminders.States
{
    internal sealed class WaitingExerciseState : ExerciseState
    {
        private IEventsService EventsService => App.ServiceProvider.GetService<IEventsService>();
        private IExerciseTimersService ExerciseTimersService => App.ServiceProvider.GetService<IExerciseTimersService>();
        private IExerciseSettingsService ExerciseSettingsService => App.ServiceProvider.GetService<IExerciseSettingsService>();
        private INotificationService NotificationService => App.ServiceProvider.GetService<INotificationService>();

        public override ExerciseStateKind Kind => ExerciseStateKind.Waiting;

        public WaitingExerciseState(ExerciseStatesContext context) : base(context) { }

        public override void Enter()
        {
            EventsService.Event += EventsService_Event;

            ITimer timer = ExerciseTimersService.GetTimer(Context.Exercise);

            timer.Loop = false;
            timer.Interval = ExerciseSettingsService.GetExerciseSettings(Context.Exercise).TimeBetweenSets;

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
            EventsService.Event -= EventsService_Event;

            ITimer timer = ExerciseTimersService.GetTimer(Context.Exercise);

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
                    ITimer timer = ExerciseTimersService.GetTimer(Context.Exercise);
                    timer.Interval = timeBetweenSetsChangedEventArgs.TimeBetweenSets;
                }
            }
        }

        private void Timer_Elapsed(object sender, EventArgs e)
        {
            ExerciseSettings exerciseSettings = ExerciseSettingsService.GetExerciseSettings(Context.Exercise);

            NotificationService.ShowReminder(new ReminderNotification()
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
