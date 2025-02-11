using Sportik.Core.Events;
using Sportik.Core.Helpers;
using Sportik.Core.Services.Interfaces;

namespace Sportik.Automation.States.Parallel
{
    internal class DisabledParallelExerciseState : ParallelExerciseState
    {
        private readonly IEventsService _eventsService;

        public override States.ParallelExerciseState ExerciseState => States.ParallelExerciseState.Disabled;

        public DisabledParallelExerciseState(ParallelExerciseStatesContext context, IEventsService eventsService) : base(context)
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
            if (CompareHelper.EqualById(Context.Exercise, args.Exercise) && args.IsEnabled)
            {
                Context.Switch(Context.WaitingExerciseState);
            }
        }
    }
}
