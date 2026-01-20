namespace Sportik.Desktop.Core.States.Training
{
    internal sealed class QueuedTrainingSetState : TrainingSetStateBase
    {
        public override TrainingSetState SetState => TrainingSetState.Queued;

        public QueuedTrainingSetState(TrainingSetStatesContext context) : base(context) { }

        protected override void HandleEnter() { }

        protected override void HandleExit() { }
    }
}
