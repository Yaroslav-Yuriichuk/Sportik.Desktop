using System;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ExerciseForceExecutionRequestedEventArgs : EventArgs
    {
        public Guid ExerciseId { get; }

        public ExerciseForceExecutionRequestedEventArgs(Guid exerciseId)
        {
            ExerciseId = exerciseId;
        }
    }
}
