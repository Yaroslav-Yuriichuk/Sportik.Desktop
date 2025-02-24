using System;

namespace Sportik.UWP.ViewModels
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
