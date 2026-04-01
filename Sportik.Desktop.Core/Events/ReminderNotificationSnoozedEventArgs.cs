using System;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ReminderNotificationSnoozedEventArgs : EventArgs
    {
        public Guid ExerciseId { get; }

        public ReminderNotificationSnoozedEventArgs(Guid exerciseId)
        {
            ExerciseId = exerciseId;
        }
    }
}