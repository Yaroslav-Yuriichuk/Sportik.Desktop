using System;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Automation.Events
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
