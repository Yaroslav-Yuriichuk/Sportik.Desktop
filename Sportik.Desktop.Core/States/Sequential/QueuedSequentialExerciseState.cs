using System.Linq;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models;
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
            if (CompareHelper.EqualById(Context.Exercise, args.Exercise) && !args.IsEnabled)
            {
                Context.Switch(Context.DisabledExerciseState);
            }
        }

        private void EventsService_Event(ExerciseSwitchRequestedEventArgs args)
        {
            if (!CompareHelper.EqualById(Context.Exercise, args.Exercise))
            {
                return;
            }

            Exercise activeExercise = Context.Exercises.FirstOrDefault(e =>
            {
                SequentialExerciseState exerciseState = Context.GetState(e);
                States.SequentialExerciseState state = exerciseState.ExerciseState;

                return state == States.SequentialExerciseState.WaitingBeforeForceExecution ||
                       state == States.SequentialExerciseState.WaitingWithForceExecution ||
                       state == States.SequentialExerciseState.Executing;
            });

            if (activeExercise != null)
            {
                SequentialExercisesStatesContext activeExercisesContext = Context.GetContext(activeExercise);
                activeExercisesContext.Switch(activeExercisesContext.QueuedExerciseState);
            }

            Context.Switch(Context.WaitingBeforeForceExecutionExerciseState);
        }
    }
}
