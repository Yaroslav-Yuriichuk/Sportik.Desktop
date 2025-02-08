using Sportik.Core.StateMachine;

namespace Sportik.Automation.States.Parallel
{
    internal abstract class ParallelExerciseState : IState
    {
        public abstract States.ParallelExerciseState ExerciseState { get; }

        protected ParallelExerciseStatesContext Context { get; }

        public ParallelExerciseState(ParallelExerciseStatesContext context)
        {
            Context = context;
        }

        public abstract void Enter();
        public abstract void Exit();
    }
}
