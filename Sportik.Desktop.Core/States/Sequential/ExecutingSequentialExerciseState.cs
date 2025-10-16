using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common.Timers;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Automation;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.Sequential
{
    internal class ExecutingSequentialExerciseState : SequentialExerciseState
    {
        private readonly IEventsService _eventsService;
        private readonly IExerciseTimersService _exerciseTimersService;
        private readonly Func<IExercisesService> _exercisesServiceFactory;

        public override States.SequentialExerciseState ExerciseState => States.SequentialExerciseState.Executing;

        public ExecutingSequentialExerciseState(SequentialExercisesStatesContext context, IEventsService eventsService,
            IExerciseTimersService exerciseTimersService, Func<IExercisesService> exercisesServiceFactory) : base(context)
        {
            _eventsService = eventsService;
            _exerciseTimersService = exerciseTimersService;
            _exercisesServiceFactory = exercisesServiceFactory;
        }

        protected override void HandleEnter()
        {
            IExercisesService exercisesService = _exercisesServiceFactory();

            _eventsService.AddListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ExerciseExecutionTimeChangedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ExerciseCompleteRequestedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ReminderNotificationDismissedEventArgs>(EventsService_Event);

            Task.Run(async () =>
            {
                Exercise exercise = await exercisesService.GetByIdAsync(Context.ExerciseId, ActiveCancellationToken);

                ITimer timer = _exerciseTimersService.GetTimer(Context.ExerciseId, ReminderMode.Sequential);

                timer.Loop = false;
                timer.Interval = exercise.Settings.ExecutionTime;

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

            ITimer timer = _exerciseTimersService.GetTimer(Context.ExerciseId, ReminderMode.Sequential);

            timer.Elapsed -= Timer_Elapsed;

            if (timer.IsRunning)
            {
                timer.Stop();
            }
        }

        private void EventsService_Event(ExerciseIsEnabledChangedEventArgs args)
        {
            if (args.ExerciseId != Context.ExerciseId || args.IsEnabled)
            {
                return;
            }

            IExercisesService exercisesService = _exercisesServiceFactory();

            Task.Run(async () =>
            {
                IEnumerable<Exercise> exercises =
                    await exercisesService.GetByIdsAsync(Context.ExerciseIds, ActiveCancellationToken);

                Exercise nextExercise = ExercisesSequenceHelper.GetNextEnabledExercise(exercises, Context.ExerciseId);

                Context.Switch(Context.DisabledExerciseState);

                if (nextExercise != null)
                {
                    SequentialExercisesStatesContext nextExerciseContext = Context.GetContext(nextExercise.Id);
                    nextExerciseContext.Switch(nextExerciseContext.WaitingBeforeForceExecutionExerciseState);
                }
            });
        }

        private void EventsService_Event(ExerciseExecutionTimeChangedEventArgs args)
        {
            if (args.ExerciseId == Context.ExerciseId)
            {
                ITimer timer = _exerciseTimersService.GetTimer(Context.ExerciseId, ReminderMode.Sequential);
                timer.Interval = args.ExecutionTime;
            }
        }

        private void EventsService_Event(ExerciseCompleteRequestedEventArgs args)
        {
            if (args.ExerciseId != Context.ExerciseId)
            {
                return;
            }

            IExercisesService exercisesService = _exercisesServiceFactory();

            Task.Run(async () =>
            {
                IEnumerable<Exercise> exercises =
                    await exercisesService.GetByIdsAsync(Context.ExerciseIds, ActiveCancellationToken);

                Exercise nextExercise = ExercisesSequenceHelper.GetNextEnabledExercise(exercises, Context.ExerciseId);

                if (nextExercise != null)
                {
                    Context.Switch(Context.QueuedExerciseState);

                    SequentialExercisesStatesContext nextExerciseContext = Context.GetContext(nextExercise.Id);
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
            if (args.ExerciseId != Context.ExerciseId)
            {
                return;
            }

            IExercisesService exercisesService = _exercisesServiceFactory();

            Task.Run(async () =>
            {
                IEnumerable<Exercise> exercises =
                    await exercisesService.GetByIdsAsync(Context.ExerciseIds, ActiveCancellationToken);

                Exercise nextExercise = ExercisesSequenceHelper.GetNextEnabledExercise(exercises, Context.ExerciseId);

                if (nextExercise != null)
                {
                    Context.Switch(Context.QueuedExerciseState);

                    SequentialExercisesStatesContext nextExerciseContext = Context.GetContext(nextExercise.Id);
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
            IExercisesService exercisesService = _exercisesServiceFactory();

            Task.Run(async () =>
            {
                IEnumerable<Exercise> exercises =
                    await exercisesService.GetByIdsAsync(Context.ExerciseIds, ActiveCancellationToken);

                Exercise nextExercise = ExercisesSequenceHelper.GetNextEnabledExercise(exercises, Context.ExerciseId);

                if (nextExercise != null)
                {
                    Context.Switch(Context.QueuedExerciseState);

                    SequentialExercisesStatesContext nextExerciseContext = Context.GetContext(nextExercise.Id);
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
