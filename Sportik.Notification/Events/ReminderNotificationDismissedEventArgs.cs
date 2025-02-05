using Sportik.Core.Models;
using System;

namespace Sportik.Notification.Events
{
    public sealed class ReminderNotificationDismissedEventArgs : EventArgs
    {
        public Exercise Exercise { get; }

        public ReminderNotificationDismissedEventArgs(Exercise exercise)
        {
            Exercise = exercise;
        }
    }
}
