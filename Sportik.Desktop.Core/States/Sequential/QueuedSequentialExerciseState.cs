using System;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.Sequential
{
    internal sealed class QueuedSequentialExerciseState : SequentialExerciseState
    {
        private readonly IEventsService _eventsService;

        public override States.SequentialExerciseState ExerciseState => States.SequentialExerciseState.Queued;

        public QueuedSequentialExerciseState(SequentialExercisesStatesContext context, IEventsService eventsService) : base(context)
        {
            _eventsService = eventsService;
        }

        protected override void HandleEnter()
        {
            _eventsService.AddListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
            _eventsService.AddListener<ExerciseSwitchRequestedEventArgs>(EventsService_Event);
        }

        protected override void HandleExit()
        {
            _eventsService.RemoveListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
            _eventsService.RemoveListener<ExerciseSwitchRequestedEventArgs>(EventsService_Event);
        }

        private void EventsService_Event(ExerciseIsEnabledChangedEventArgs args)
        {
            if (args.ExerciseId == Context.ExerciseId && !args.IsEnabled)
            {
                Context.Switch(Context.DisabledExerciseState);
            }
        }

        private void EventsService_Event(ExerciseSwitchRequestedEventArgs args)
        {
            if (args.ExerciseId != Context.ExerciseId)
            {
                return;
            }

            foreach (Guid exerciseId in Context.ExerciseIds)
            {
                SequentialExerciseState exerciseState = Context.GetState(exerciseId);
                States.SequentialExerciseState state = exerciseState.ExerciseState;

                bool isActive = state == States.SequentialExerciseState.WaitingBeforeForceExecution ||
                       state == States.SequentialExerciseState.WaitingWithForceExecution ||
                       state == States.SequentialExerciseState.Executing;

                if (isActive)
                {
                    SequentialExercisesStatesContext activeExercisesContext = Context.GetContext(exerciseId);
                    activeExercisesContext.Switch(activeExercisesContext.QueuedExerciseState);
                }
            }

            Context.Switch(Context.WaitingBeforeForceExecutionExerciseState);
        }
    }
}
