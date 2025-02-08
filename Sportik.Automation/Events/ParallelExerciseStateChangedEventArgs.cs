using System;
using Sportik.Automation.States;
using Sportik.Core.Models;

namespace Sportik.Automation.Events
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
