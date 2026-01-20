using System.Threading;
using Sportik.Desktop.Core.Common.StateMachine;

namespace Sportik.Desktop.Core.States.Training
{
    internal abstract class TrainingSetStateBase : IState
    {
        public abstract TrainingSetState SetState { get; }

        protected TrainingSetStatesContext Context { get; }

        protected CancellationToken ActiveCancellationToken => _activeCts?.Token ?? new CancellationToken(true);

        private CancellationTokenSource _activeCts;

        protected TrainingSetStateBase(TrainingSetStatesContext context)
        {
            Context = context;
        }

        public void Enter()
        {
            _activeCts?.Cancel();
            _activeCts = new CancellationTokenSource();

            HandleEnter();
        }

        public void Exit()
        {
            _activeCts?.Cancel();
            _activeCts = null;

            HandleExit();
        }

        protected abstract void HandleEnter();
        protected abstract void HandleExit();
    }
}
