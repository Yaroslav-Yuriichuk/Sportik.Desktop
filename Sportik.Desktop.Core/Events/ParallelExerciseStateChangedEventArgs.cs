using System;
using Sportik.Desktop.Core.States;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ParallelExerciseStateChangedEventArgs : EventArgs
    {
        public Guid ExerciseId { get; }

        public ParallelExerciseState PreviousState { get; }

        public ParallelExerciseState CurrentState { get; }

        public ParallelExerciseStateChangedEventArgs(Guid exerciseId, ParallelExerciseState previousState, ParallelExerciseState currentState)
        {
            ExerciseId = exerciseId;
            PreviousState = previousState;
            CurrentState = currentState;
        }
    }
}
