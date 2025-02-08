using Sportik.Core.Events;
using Sportik.Core.Helpers;
using Sportik.Core.Services.Interfaces;

namespace Sportik.Automation.States.Sequential
{
    internal sealed class QueuedSequentialExerciseState : SequentialExerciseState
    {
        private readonly IEventsService _eventsService;

        public override States.SequentialExerciseState ExerciseState => States.SequentialExerciseState.Queued;

        public QueuedSequentialExerciseState(SequentialExercisesStatesContext context, IEventsService eventsService) : base(context)
        {
            _eventsService = eventsService;
        }

        public override void Enter()
        {
            _eventsService.AddListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
        }

        public override void Exit()
        {
            _eventsService.RemoveListener<ExerciseIsEnabledChangedEventArgs>(EventsService_Event);
        }

        private void EventsService_Event(ExerciseIsEnabledChangedEventArgs args)
        {
            if (CompareHelper.EqualById(Context.Exercise, args.Exercise) && !args.IsEnabled)
            {
                Context.Switch(Context.DisabledExerciseState);
            }
        }
    }
}
