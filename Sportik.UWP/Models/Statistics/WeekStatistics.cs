using System.Collections.Generic;

namespace Sportik.UWP.Models.Statistics
{
    internal sealed class WeekStatistics
    {
        public List<DayStatistics> DayStatistics { get; set; } = new List<DayStatistics>();
    }
}
