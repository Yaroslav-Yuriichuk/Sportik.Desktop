using System;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ReminderNotificationDismissedEventArgs : EventArgs
    {
        public Guid ExerciseId { get; }

        public ReminderNotificationDismissedEventArgs(Guid exerciseId)
        {
            ExerciseId = exerciseId;
        }
    }
}
