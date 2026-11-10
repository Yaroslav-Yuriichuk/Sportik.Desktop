using System;

namespace Sportik.Desktop.Core.Models
{
    public sealed class GeneralNotification
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public TimeSpan DisplayTime { get; set; } = TimeSpan.FromSeconds(5);
    }
}