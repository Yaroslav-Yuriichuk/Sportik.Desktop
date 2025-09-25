using System;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Notification.Events
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
