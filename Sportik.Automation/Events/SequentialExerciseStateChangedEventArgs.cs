using Sportik.Core.Models;
using System;
using Sportik.Automation.States;

namespace Sportik.Automation.Events
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
