using System;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ExerciseIsEnabledChangedEventArgs : EventArgs
    {
        public Guid ExerciseId { get; }

        public bool IsEnabled { get; }

        public ExerciseIsEnabledChangedEventArgs(Guid exerciseId, bool isEnabled)
        {
            ExerciseId = exerciseId;
            IsEnabled = isEnabled;
        }
    }
}
