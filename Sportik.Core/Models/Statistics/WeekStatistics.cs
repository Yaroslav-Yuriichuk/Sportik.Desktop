using System.Collections.Generic;

namespace Sportik.Core.Models.Statistics
{
    public sealed class WeekStatistics
    {
        public List<DayStatistics> DayStatistics { get; set; } = new List<DayStatistics>();
    }
}
