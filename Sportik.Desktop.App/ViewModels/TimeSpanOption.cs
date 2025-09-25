using System;

namespace Sportik.Desktop.App.ViewModels
{
    internal sealed class TimeSpanOption
    {
        public TimeSpan TimeSpanValue { get; set; }

        public TimeSpanOption(TimeSpan timeSpanValue)
        {
            TimeSpanValue = timeSpanValue;
        }
    }
}
