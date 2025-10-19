using System;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ExerciseSwitchRequestedEventArgs : EventArgs
    {
        public Guid ExerciseId { get; }

        public ExerciseSwitchRequestedEventArgs(Guid exerciseId)
        {
            ExerciseId = exerciseId;
        }
    }
}
