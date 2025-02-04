using System;
using Sportik.Automation.States;
using Sportik.Core.Models;

namespace Sportik.Automation.Events
{
    public class ExerciseStateChangedEventArgs : EventArgs
    {
        public Exercise Exercise { get; }

        public ExerciseStateKind PreviousState { get; }

        public ExerciseStateKind CurrentState { get; }

        public ExerciseStateChangedEventArgs(Exercise exercise, ExerciseStateKind previousState, ExerciseStateKind currentState)
        {
            Exercise = exercise;
            PreviousState = previousState;
            CurrentState = currentState;
        }
    }
}
