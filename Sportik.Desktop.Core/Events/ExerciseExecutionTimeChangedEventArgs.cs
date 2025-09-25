using System;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ExerciseExecutionTimeChangedEventArgs : EventArgs
    {
        public Exercise Exercise { get; }

        public TimeSpan ExecutionTime { get; }

        public ExerciseExecutionTimeChangedEventArgs(Exercise exercise, TimeSpan executionTime)
        {
            Exercise = exercise;
            ExecutionTime = executionTime;
        }
    }
}
