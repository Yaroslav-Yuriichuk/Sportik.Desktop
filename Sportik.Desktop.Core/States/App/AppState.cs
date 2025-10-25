using Sportik.Desktop.Core.Common.StateMachine;
using System.Threading;

namespace Sportik.Desktop.Core.States.App
{
    internal abstract class AppState : IState
    {
        public abstract ApplicationState ApplicationState { get; }

        protected AppStatesContext Context { get; }

        protected CancellationToken ActiveCancellationToken => _activeCts?.Token ?? new CancellationToken(true);

        private CancellationTokenSource _activeCts;

        public AppState(AppStatesContext context)
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
