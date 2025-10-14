using System;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Events
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
