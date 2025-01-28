using System;

namespace Sportik.UWP.Models.Notifications
{
    internal sealed class ReminderNotification
    {
        public string Title { get; set; }

        public string[] Texts { get; set; }

        public TimeSpan ExpirationTime { get; set; }
    }
}
