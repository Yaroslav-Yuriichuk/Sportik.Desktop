using System;
using Sportik.Desktop.Core.States;
using Sportik.Desktop.Core.States.Exercises;

namespace Sportik.Desktop.Core.Events
{
    public sealed class SequentialExerciseStateChangedEventArgs : EventArgs
    {
        public Guid ExerciseId { get; }

        public SequentialExerciseState PreviousState { get; }

        public SequentialExerciseState CurrentState { get; }

        public SequentialExerciseStateChangedEventArgs(Guid exerciseId, SequentialExerciseState previousState, SequentialExerciseState currentState)
        {
            ExerciseId = exerciseId;
            PreviousState = previousState;
            CurrentState = currentState;
        }
    }
}
