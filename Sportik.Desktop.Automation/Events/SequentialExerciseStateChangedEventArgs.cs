using System;
using Sportik.Desktop.Automation.States;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Automation.Events
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
