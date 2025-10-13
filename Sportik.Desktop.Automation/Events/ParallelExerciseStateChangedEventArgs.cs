using System;
using Sportik.Desktop.Automation.States;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Automation.Events
{
    public class ParallelExerciseStateChangedEventArgs : EventArgs
    {
        public Exercise Exercise { get; }

        public ParallelExerciseState PreviousState { get; }

        public ParallelExerciseState CurrentState { get; }

        public ParallelExerciseStateChangedEventArgs(Exercise exercise, ParallelExerciseState previousState, ParallelExerciseState currentState)
        {
            Exercise = exercise;
            PreviousState = previousState;
            CurrentState = currentState;
        }
    }
}
