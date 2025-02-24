using System;
using Sportik.Core.Models;

namespace Sportik.Automation.Events
{
    public sealed class ExerciseForceExecutionRequestedEventArgs : EventArgs
    {
        public Exercise Exercise { get; }

        public ExerciseForceExecutionRequestedEventArgs(Exercise exercise)
        {
            Exercise = exercise;
        }
    }
}
