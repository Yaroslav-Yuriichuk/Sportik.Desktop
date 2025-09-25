using Sportik.Desktop.Automation.States.Parallel;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.StateMachine;

namespace Sportik.Desktop.Automation.States.Sequential
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
