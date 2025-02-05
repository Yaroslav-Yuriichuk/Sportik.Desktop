using Sportik.Core.Models;
using System;

namespace Sportik.Notification.Events
{
    public sealed class ReminderNotificationShownEventArgs : EventArgs
    {
        public Exercise Exercise { get; }

        public ReminderNotificationShownEventArgs(Exercise exercise)
        {
            Exercise = exercise;
        }
    }
}
