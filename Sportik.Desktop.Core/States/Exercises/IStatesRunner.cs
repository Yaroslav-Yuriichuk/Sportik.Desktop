using System;
using Sportik.Desktop.Core.Models.Automation;

namespace Sportik.Desktop.Core.States.Exercises
{
    internal interface IStatesRunner : IDisposable
    {
        public ReminderMode Mode { get; }

        TState GetExerciseState<TState>(Guid exerciseId) where TState : Enum;

        void AddExercise(Guid exerciseId);

        void RemoveExercise(Guid exerciseId);
    }
}
