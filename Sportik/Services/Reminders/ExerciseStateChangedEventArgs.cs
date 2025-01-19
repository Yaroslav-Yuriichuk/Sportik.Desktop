using System;
using Sportik.Models;

namespace Sportik.Services.Reminders
{
    internal class ExerciseStateChangedEventArgs : EventArgs
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
