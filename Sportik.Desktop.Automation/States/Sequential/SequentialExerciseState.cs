using System.Threading;
using Sportik.Desktop.Core.StateMachine;

namespace Sportik.Desktop.Automation.States.Sequential
{
    internal abstract class SequentialExerciseState : IState
    {
        public abstract States.SequentialExerciseState ExerciseState { get; }

        protected SequentialExercisesStatesContext Context { get; }

        protected CancellationToken ActiveCancellationToken => _activeCts?.Token ?? new CancellationToken(true);

        private CancellationTokenSource _activeCts;

        public SequentialExerciseState(SequentialExercisesStatesContext context)
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
