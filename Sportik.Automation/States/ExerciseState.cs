using Sportik.Automation.States;
using Sportik.Core.StateMachine;

namespace Sportik.UWP.Services.Reminders.States
{
    internal abstract class ExerciseState : IState
    {
        public abstract ExerciseStateKind Kind { get; }

        protected ExerciseStatesContext Context { get; }

        public ExerciseState(ExerciseStatesContext context)
        {
            Context = context;
        }

        public abstract void Enter();
        public abstract void Exit();
    }
}
