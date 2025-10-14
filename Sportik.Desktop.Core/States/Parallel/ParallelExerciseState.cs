using System.Threading;
using Sportik.Desktop.Core.Common.StateMachine;

namespace Sportik.Desktop.Core.States.Parallel
{
    internal abstract class ParallelExerciseState : IState
    {
        public abstract States.ParallelExerciseState ExerciseState { get; }

        protected ParallelExerciseStatesContext Context { get; }

        protected CancellationToken ActiveCancellationToken => _activeCts?.Token ?? new CancellationToken(true);

        private CancellationTokenSource _activeCts;

        public ParallelExerciseState(ParallelExerciseStatesContext context)
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
