using System;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ReminderNotificationShownEventArgs : EventArgs
    {
        public Guid ExerciseId { get; }

        public ReminderNotificationShownEventArgs(Guid exerciseId)
        {
            ExerciseId = exerciseId;
        }
    }
}
