using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sportik.Desktop.Automation.Events;
using Sportik.Desktop.Automation.Helpers;
using Sportik.Desktop.Automation.Models;
using Sportik.Desktop.Automation.Services;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Core.Timers;
using Sportik.Desktop.Notification.Events;

namespace Sportik.Desktop.Automation.States.Sequential
{
    internal class ExecutingSequentialExerciseState : SequentialExerciseState
    {
        private readonly IEventsService _eventsService;
        private readonly IExerciseTimersService _exerciseTimersService;
        private readonly Func<IExerciseSettingsService> _exerciseSettingsServiceFactory;

        public override States.SequentialExerciseState ExerciseState => States.SequentialExerciseState.Executing;

        public ExecutingSequentialExerciseState(SequentialExercisesStatesContext context, IEventsService eventsService, IExerciseTimersService exerciseTimersService,
            Func<IExerciseSettingsService> exerciseSettingsServiceFactory) : base(context)
        {
            _eventsService = eventsService;
            _exerciseTimersService = exerciseTimersService;
            _exerciseSettingsServiceFactory = exerciseSettingsServiceFactory;
        }

        protected override void HandleEnter()
        {
            IExerciseSettingsService exerciseSettingsService = _exerciseSettingsServiceFactory();

            _eventsService.AddListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ExerciseExecutionTimeChangedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ExerciseCompleteRequestedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ReminderNotificationDismissedEventArgs>(EventsService_Event);

            Task.Run(async () =>
            {
                ExerciseSettings exerciseSettings =
                    await exerciseSettingsService.GetExerciseSettingsAsync(Context.Exercise, ActiveCancellationToken);

                ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise, ReminderMode.Sequential);

                timer.Loop = false;
                timer.Interval = exerciseSettings.ExecutionTime;

                timer.Elapsed += Timer_Elapsed;

                if (!timer.IsRunning)
                {
                    timer.Start();
                }
            });
        }

        protected override void HandleExit()
        {
            _eventsService.RemoveListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<ExerciseExecutionTimeChangedEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<ExerciseCompleteRequestedEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<ReminderNotificationDismissedEventArgs>(EventsService_Event);

            ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise, ReminderMode.Sequential);

            timer.Elapsed -= Timer_Elapsed;

            if (timer.IsRunning)
            {
                timer.Stop();
            }
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

        private void EventsService_Event(ExerciseExecutionTimeChangedEventArgs args)
        {
            if (CompareHelper.EqualById(Context.Exercise, args.Exercise))
            {
                ITimer timer = _exerciseTimersService.GetTimer(Context.Exercise, ReminderMode.Sequential);
                timer.Interval = args.ExecutionTime;
            }
        }

        private void EventsService_Event(ExerciseCompleteRequestedEventArgs args)
        {
            if (!CompareHelper.EqualById(Context.Exercise, args.Exercise))
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

                if (nextExercise != null)
                {
                    Context.Switch(Context.QueuedExerciseState);

                    SequentialExercisesStatesContext nextExerciseContext = Context.GetContext(nextExercise);
                    nextExerciseContext.Switch(nextExerciseContext.WaitingBeforeForceExecutionExerciseState);
                }
                else
                {
                    Context.Switch(Context.WaitingBeforeForceExecutionExerciseState);
                }
            });
        }

        private void EventsService_Event(ReminderNotificationDismissedEventArgs args)
        {
            if (!CompareHelper.EqualById(Context.Exercise, args.Exercise))
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

                if (nextExercise != null)
                {
                    Context.Switch(Context.QueuedExerciseState);

                    SequentialExercisesStatesContext nextExerciseContext = Context.GetContext(nextExercise);
                    nextExerciseContext.Switch(nextExerciseContext.WaitingBeforeForceExecutionExerciseState);
                }
                else
                {
                    Context.Switch(Context.WaitingBeforeForceExecutionExerciseState);
                }
            });
        }

        private void Timer_Elapsed(object sender, EventArgs args)
        {
            IExerciseSettingsService exerciseSettingsService = _exerciseSettingsServiceFactory();

            Task.Run(async () =>
            {
                IEnumerable<ExerciseSettings> exerciseSettings = await Task.WhenAll(
                    Context.Exercises.Select(async e =>
                        await exerciseSettingsService.GetExerciseSettingsAsync(e, ActiveCancellationToken)));

                Exercise nextExercise = ExercisesSequenceHelper.GetNextEnabledExercise(Context.Exercises, Context.Exercise, exerciseSettings);

                if (nextExercise != null)
                {
                    Context.Switch(Context.QueuedExerciseState);

                    SequentialExercisesStatesContext nextExerciseContext = Context.GetContext(nextExercise);
                    nextExerciseContext.Switch(nextExerciseContext.WaitingBeforeForceExecutionExerciseState);
                }
                else
                {
                    Context.Switch(Context.WaitingBeforeForceExecutionExerciseState);
                }
            });
        }
    }
}
