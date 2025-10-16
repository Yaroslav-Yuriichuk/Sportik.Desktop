using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.Sequential
{
    internal sealed class DisabledSequentialExerciseState : SequentialExerciseState
    {
        private readonly IEventsService _eventsService;
        private readonly Func<IExercisesService> _exercisesServiceFactory;

        public override States.SequentialExerciseState ExerciseState => States.SequentialExerciseState.Disabled;

        public DisabledSequentialExerciseState(SequentialExercisesStatesContext context, IEventsService eventsService,
            Func<IExercisesService> exercisesServiceFactory) : base(context)
        {
            _eventsService = eventsService;
            _exercisesServiceFactory = exercisesServiceFactory;
        }

        protected override void HandleEnter()
        {
            _eventsService.AddListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
        }

        protected override void HandleExit()
        {
            _eventsService.RemoveListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
        }

        private void EventsService_Event(ExerciseIsEnabledChangedEventArgs args)
        {
            if (args.ExerciseId != Context.ExerciseId || !args.IsEnabled)
            {
                return;
            }

            IExercisesService exercisesService = _exercisesServiceFactory();

            Task.Run(async () =>
            {
                IEnumerable<Exercise> exercises = await exercisesService.GetByIdsAsync(Context.ExerciseIds, ActiveCancellationToken);

                Exercise otherExercise = ExercisesSequenceHelper.GetAnyEnabledExercise(exercises.Where(e => e.Id != Context.ExerciseId));

                Context.Switch(otherExercise == null
                    ? Context.WaitingBeforeForceExecutionExerciseState
                    : Context.QueuedExerciseState);
            });
        }
    }
}
