using System;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ReminderNotificationAcceptedEventArgs : EventArgs
    {
        public Guid ExerciseId { get; }

        public ReminderNotificationAcceptedEventArgs(Guid exerciseId)
        {
            ExerciseId = exerciseId;
        }
    }
}
