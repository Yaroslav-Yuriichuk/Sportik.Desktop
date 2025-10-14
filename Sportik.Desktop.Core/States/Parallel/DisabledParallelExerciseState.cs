using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.Parallel
{
    internal class DisabledParallelExerciseState : ParallelExerciseState
    {
        private readonly IEventsService _eventsService;

        public override States.ParallelExerciseState ExerciseState => States.ParallelExerciseState.Disabled;

        public DisabledParallelExerciseState(ParallelExerciseStatesContext context, IEventsService eventsService) : base(context)
        {
            _eventsService = eventsService;
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
            if (CompareHelper.EqualById(Context.Exercise, args.Exercise) && args.IsEnabled)
            {
                Context.Switch(Context.WaitingBeforeForceExecutionExerciseState);
            }
        }
    }
}
