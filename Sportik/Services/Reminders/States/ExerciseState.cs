using Sportik.Core;

namespace Sportik.Services.Reminders.States
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
