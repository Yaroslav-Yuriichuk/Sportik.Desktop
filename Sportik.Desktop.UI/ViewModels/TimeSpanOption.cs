using System;

namespace Sportik.Desktop.UI.ViewModels
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
