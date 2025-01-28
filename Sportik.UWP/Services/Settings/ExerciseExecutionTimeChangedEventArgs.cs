using System;
using Sportik.UWP.Models;

namespace Sportik.UWP.Services.Settings
{
    internal sealed class ExerciseExecutionTimeChangedEventArgs : EventArgs
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
