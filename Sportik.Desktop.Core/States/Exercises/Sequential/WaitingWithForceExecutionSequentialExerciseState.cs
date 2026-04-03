using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Common.Timers;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Automation;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.Exercises.Sequential
{
    internal sealed class WaitingWithForceExecutionSequentialExerciseState : SequentialExerciseState
    {
        private readonly IEventsService _eventsService;
        private readonly IExerciseTimersService _exerciseTimersService;
        private readonly Func<IExercisesService> _exercisesServiceFactory;

        public override Exercises.SequentialExerciseState ExerciseState => Exercises.SequentialExerciseState.WaitingWithForceExecution;

        public WaitingWithForceExecutionSequentialExerciseState(SequentialExercisesStatesContext context, IEventsService eventsService,
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
            _eventsService.AddListener<ExerciseTimeBetweenSetsChangedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ExerciseForceExecutionRequestedEventArgs>(EventsService_Event);

            Task.Run(async () =>
            {
                OperationResult<Exercise> result = await exercisesService.GetByIdAsync(Context.ExerciseId, ActiveCancellationToken);

                if (!result.Succeeded)
                {
                    // TODO: Handle error.
                    return;
                }

                ITimer timer = _exerciseTimersService.GetTimer(Context.ExerciseId, ReminderMode.Sequential);

                timer.Loop = false;
                timer.Interval = result.Value.Settings.TimeBetweenSets;

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

            ITimer timer = _exerciseTimersService.GetTimer(Context.ExerciseId, ReminderMode.Sequential);

            timer.Elapsed -= Timer_Elapsed;

            if (timer.IsRunning)
            {
                timer.Pause();
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
                OperationResult<IEnumerable<Exercise>> result = await exercisesService.GetByIdsAsync(Context.ExerciseIds, ActiveCancellationToken);

                if (!result.Succeeded)
                {
                    // TODO: Handle error.
                    return;
                }

                Exercise nextExercise = ExercisesSequenceHelper.GetNextEnabledExercise(result.Value, Context.ExerciseId);

                Context.Switch(Context.DisabledExerciseState);

                if (nextExercise != null)
                {
                    SequentialExercisesStatesContext nextExerciseContext = Context.GetContext(nextExercise.Id);
                    nextExerciseContext.Switch(nextExerciseContext.WaitingBeforeForceExecutionExerciseState);
                }
            });
        }

        private void EventsService_Event(ExerciseTimeBetweenSetsChangedEventArgs args)
        {
            if (args.ExerciseId == Context.ExerciseId)
            {
                ITimer timer = _exerciseTimersService.GetTimer(Context.ExerciseId, ReminderMode.Sequential);
                timer.Interval = args.TimeBetweenSets;
            }
        }

        private void EventsService_Event(ExerciseForceExecutionRequestedEventArgs args)
        {
            if (args.ExerciseId == Context.ExerciseId)
            {
                Context.Switch(Context.ExecutingExerciseState);
            }
        }

        private void Timer_Elapsed(object sender, EventArgs args)
        {
            Context.Switch(Context.ExecutingExerciseState);
        }
    }
}
