using System.Collections.Generic;

namespace Sportik.Desktop.Core.Models.Statistics
{
    public sealed class WeekStatistics
    {
        public List<DayStatistics> DayStatistics { get; }

        public WeekStatistics(List<DayStatistics> dayStatistics)
        {
            DayStatistics = dayStatistics;
        }
    }
}
