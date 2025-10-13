using System;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.States;

namespace Sportik.Desktop.Core.Events
{
    public sealed class SequentialExerciseStateChangedEventArgs : EventArgs
    {
        public Exercise Exercise { get; }

        public SequentialExerciseState PreviousState { get; }

        public SequentialExerciseState CurrentState { get; }

        public SequentialExerciseStateChangedEventArgs(Exercise exercise, SequentialExerciseState previousState, SequentialExerciseState currentState)
        {
            Exercise = exercise;
            PreviousState = previousState;
            CurrentState = currentState;
        }
    }
}
