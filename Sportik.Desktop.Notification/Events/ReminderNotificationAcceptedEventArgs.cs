using System;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Notification.Events
{
    public sealed class ReminderNotificationAcceptedEventArgs : EventArgs
    {
        public Exercise Exercise { get; }

        public ReminderNotificationAcceptedEventArgs(Exercise exercise)
        {
            Exercise = exercise;
        }
    }
}
