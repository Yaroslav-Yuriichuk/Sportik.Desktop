using System;

namespace Sportik.Notification.Models
{
    public sealed class ReminderNotification
    {
        public string Title { get; set; }

        public string[] Texts { get; set; }

        public TimeSpan ExpirationTime { get; set; }
    }
}
