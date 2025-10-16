using System;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ExerciseExecutionTimeChangedEventArgs : EventArgs
    {
        public Guid ExerciseId { get; }

        public TimeSpan ExecutionTime { get; }

        public ExerciseExecutionTimeChangedEventArgs(Guid exerciseId, TimeSpan executionTime)
        {
            ExerciseId = exerciseId;
            ExecutionTime = executionTime;
        }
    }
}
