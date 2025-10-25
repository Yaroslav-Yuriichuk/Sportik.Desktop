using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.Exercises.Parallel
{
    internal class DisabledParallelExerciseState : ParallelExerciseState
    {
        private readonly IEventsService _eventsService;

        public override Exercises.ParallelExerciseState ExerciseState => Exercises.ParallelExerciseState.Disabled;

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
            if (args.ExerciseId == Context.ExerciseId && args.IsEnabled)
            {
                Context.Switch(Context.WaitingBeforeForceExecutionExerciseState);
            }
        }
    }
}
