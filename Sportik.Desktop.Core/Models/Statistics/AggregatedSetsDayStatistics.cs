using System;

namespace Sportik.Desktop.Core.Models.Statistics
{
    public sealed class AggregatedSetsDayStatistics
    {
        public DayOfWeek DayOfWeek { get; }

        public int TotalSets { get; }

        public AggregatedSetsDayStatistics(DayOfWeek dayOfWeek, int totalSets)
        {
            DayOfWeek = dayOfWeek;
            TotalSets = totalSets;
        }
    }
}