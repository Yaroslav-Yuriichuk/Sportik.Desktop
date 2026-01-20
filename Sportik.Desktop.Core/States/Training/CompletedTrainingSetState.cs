namespace Sportik.Desktop.Core.States.Training
{
    internal sealed class CompletedTrainingSetState : TrainingSetStateBase
    {
        public override TrainingSetState SetState => TrainingSetState.Completed;

        public CompletedTrainingSetState(TrainingSetStatesContext context) : base(context) { }

        protected override void HandleEnter() { }

        protected override void HandleExit() { }
    }
}
