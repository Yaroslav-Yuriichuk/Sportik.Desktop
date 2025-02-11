using Sportik.Automation.States.Parallel;
using Sportik.Core.Models;
using Sportik.Core.StateMachine;

namespace Sportik.Automation.States.Sequential
{
    internal abstract class SequentialExerciseState : IState
    {
        public abstract States.SequentialExerciseState ExerciseState { get; }

        protected SequentialExercisesStatesContext Context { get; }

        public SequentialExerciseState(SequentialExercisesStatesContext context)
        {
            Context = context;
        }

        public abstract void Enter();
        public abstract void Exit();
    }
}
