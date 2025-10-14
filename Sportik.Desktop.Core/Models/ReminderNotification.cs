using System;

namespace Sportik.Desktop.Core.Models
{
    public sealed class ReminderNotification
    {
        public string Title { get; set; }

        public string[] Texts { get; set; }

        public TimeSpan ExpirationTime { get; set; }
    }
}
