using System;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Core.Timers;

namespace Sportik.Desktop.Core.States.Parallel
{
    internal sealed class WaitingWithForceExecutionParallelExerciseState : ParallelExerciseState
    {
        private readonly IEventsService _eventsService;
        private readonly IExerciseTimersService _exerciseTimersService;
        private readonly Func<IExerciseSettingsService> _exerciseSettingsServiceFactory;
        private readonly Func<INotificationService> _notificationServiceFactory;

        public override States.ParallelExerciseState ExerciseState => States.ParallelExerciseState.WaitingWithForceExecution;

        public WaitingWithForceExecutionParallelExerciseState(ParallelExerciseStatesContext context, IEventsService eventsService, IExerciseTimersService exerciseTimersService,
            Func<IExerciseSettingsService> exerciseSettingsServiceFactory, Func<INotificationService> notificationServiceFactory) : base(context)
        {
            _eventsService = eventsService;
            _exerciseTimersService = exerciseTimersService;
            _exerciseSettingsServiceFactory = exerciseSettingsServiceFactory;
            _notificationServiceFactory = notificationServiceFactory;
        }

        protected override void HandleEnter()
        {
            IExerciseSettingsService exerciseSettingsService = _exerciseSettingsServiceFactory();

            _eventsService.AddListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ExerciseTimeBetweenSetsChangedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ExerciseForceExecutionRequestedEventArgs>(EventsService_Event);

            Task.Run(async () =>
            {
                ExerciseSettings exerciseSettings =
                    await exerciseSettingsService.GetExerciseSettingsAsync(Context.Exercise, ActiveCancellationToken);

                ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise, ReminderMode.Parallel);

                timer.Loop = false;
                timer.Interval = exerciseSettings.TimeBetweenSets;

                timer.Elapsed += Timer_Elapsed;

                if (timer.IsPaused)
                {
                    timer.Resume();
                }
                else
                {
                    timer.Start();
                }
            });
        }

        protected override void HandleExit()
        {
            _eventsService.RemoveListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<ExerciseTimeBetweenSetsChangedEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<ExerciseForceExecutionRequestedEventArgs>(EventsService_Event);

            ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise, ReminderMode.Parallel);

            timer.Elapsed -= Timer_Elapsed;

            if (timer.IsRunning)
            {
                timer.Pause();
            }
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

        private void EventsService_Event(ExerciseForceExecutionRequestedEventArgs args)
        {
            if (CompareHelper.EqualById(Context.Exercise, args.Exercise))
            {
                Context.Switch(Context.ExecutingExerciseState);
            }
        }

        private void Timer_Elapsed(object sender, EventArgs args)
        {
            IExerciseSettingsService exerciseSettingsService = _exerciseSettingsServiceFactory();
            INotificationService notificationService = _notificationServiceFactory();

            Task.Run(async () =>
            {
                ExerciseSettings exerciseSettings =
                    await exerciseSettingsService.GetExerciseSettingsAsync(Context.Exercise, ActiveCancellationToken);

                notificationService.ShowReminder(Context.Exercise, new ReminderNotification
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
            });
        }
    }
}
