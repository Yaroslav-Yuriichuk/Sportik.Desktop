using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Extensions;
using Sportik.Desktop.Automation.Constants;
using Sportik.Desktop.Automation.Helpers;
using Sportik.Desktop.Automation.Models;
using Sportik.Desktop.Automation.Services;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Core.Timers;
using Sportik.Desktop.Notification.Models;
using Sportik.Desktop.Notification.Services;

namespace Sportik.Desktop.Automation.States.Sequential
{
    internal sealed class WaitingBeforeForceExecutionSequentialExerciseState : SequentialExerciseState
    {
        private readonly IEventsService _eventsService;
        private readonly IExerciseTimersService _exerciseTimersService;
        private readonly IRuntimeCacheService _runtimeCacheService;
        private readonly Func<IExerciseSettingsService> _exerciseSettingsServiceFactory;
        private readonly Func<INotificationService> _notificationServiceFactory;

        public override States.SequentialExerciseState ExerciseState => States.SequentialExerciseState.WaitingBeforeForceExecution;

        private ITimer _forceExecutionTimer;

        public WaitingBeforeForceExecutionSequentialExerciseState(SequentialExercisesStatesContext context, IEventsService eventsService, IExerciseTimersService exerciseTimersService,
            IRuntimeCacheService runtimeCacheService, Func<IExerciseSettingsService> exerciseSettingsServiceFactory, Func<INotificationService> notificationServiceFactory) : base(context)
        {
            _eventsService = eventsService;
            _exerciseTimersService = exerciseTimersService;
            _runtimeCacheService = runtimeCacheService;
            _exerciseSettingsServiceFactory = exerciseSettingsServiceFactory;
            _notificationServiceFactory = notificationServiceFactory;
        }

        protected override void HandleEnter()
        {
            SequentialExercisesCache sequentialExercisesCache = _runtimeCacheService.GetOrNew<SequentialExercisesCache>();
            sequentialExercisesCache.LastActiveExerciseId = Context.Exercise.Id;

            _runtimeCacheService.Set(sequentialExercisesCache);

            IExerciseSettingsService exerciseSettingsService = _exerciseSettingsServiceFactory();

            _eventsService.AddListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ExerciseTimeBetweenSetsChangedEventArgs>(EventsService_Event);

            Task.Run(async () =>
            {
                ExerciseSettings exerciseSettings =
                    await exerciseSettingsService.GetExerciseSettingsAsync(Context.Exercise, ActiveCancellationToken);

                ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise, ReminderMode.Sequential);

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

                _forceExecutionTimer = new DefaultTimerBuilder()
                    .SetInterval(AutomationConstants.TimeBeforeForceExecution)
                    .SetCallback(ForceExecutionTimer_Elapsed)
                    .Build();

                _forceExecutionTimer.Start();
            });
        }

        protected override void HandleExit()
        {
            _eventsService.RemoveListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<ExerciseTimeBetweenSetsChangedEventArgs>(EventsService_Event);

            ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise, ReminderMode.Sequential);

            timer.Elapsed -= Timer_Elapsed;

            if (timer.IsRunning)
            {
                timer.Pause();
            }

            _forceExecutionTimer.Stop();
        }

        private void EventsService_Event(ExerciseIsEnabledChangedEventArgs args)
        {
            if (!CompareHelper.EqualById(Context.Exercise, args.Exercise) || args.IsEnabled)
            {
                return;
            }

            IExerciseSettingsService exerciseSettingsService = _exerciseSettingsServiceFactory();

            Task.Run(async () =>
            {
                IEnumerable<ExerciseSettings> exerciseSettings = await Task.WhenAll(
                    Context.Exercises.Select(async e =>
                        await exerciseSettingsService.GetExerciseSettingsAsync(e, ActiveCancellationToken)));

                Exercise nextExercise = ExercisesSequenceHelper.GetNextEnabledExercise(Context.Exercises, Context.Exercise, exerciseSettings);

                Context.Switch(Context.DisabledExerciseState);

                if (nextExercise != null)
                {
                    SequentialExercisesStatesContext nextExerciseContext = Context.GetContext(nextExercise);
                    nextExerciseContext.Switch(nextExerciseContext.WaitingBeforeForceExecutionExerciseState);
                }
            });
        }

        private void EventsService_Event(ExerciseTimeBetweenSetsChangedEventArgs args)
        {
            if (CompareHelper.EqualById(Context.Exercise, args.Exercise))
            {
                ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise, ReminderMode.Sequential);
                timer.Interval = args.TimeBetweenSets;
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
                    Texts = new[]
                    {
                        $"You have {exerciseSettings.ExecutionTime.TotalMinutes} minutes to complete {Context.Exercise.Name.ToLower()} exercise.",
                        $"Target repetitions: {exerciseSettings.TargetRepetitions}.",
                    },
                    ExpirationTime = exerciseSettings.ExecutionTime,
                });

                Context.Switch(Context.ExecutingExerciseState);
            });
        }

        private void ForceExecutionTimer_Elapsed(object sender, EventArgs e)
        {
            Context.Switch(Context.WaitingWithForceExecutionExerciseState);
        }
    }
}
