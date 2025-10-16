using System;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ExerciseCompleteRequestedEventArgs : EventArgs
    {
        public Guid ExerciseId { get; }

        public ExerciseCompleteRequestedEventArgs(Guid exerciseId)
        {
            ExerciseId = exerciseId;
        }
    }
}
