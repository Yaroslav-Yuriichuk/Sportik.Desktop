using Sportik.Core.Models;
using System;

namespace Sportik.Notification.Events
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
