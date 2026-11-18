using System;

namespace Sportik.Desktop.Core.Models.Statistics
{
    public sealed class AggregatedSetsDayOfYearStatistics
    {
        public DateTime Date { get; }

        public int TotalSets { get; }

        public AggregatedSetsDayOfYearStatistics(DateTime date, int totalSets)
        {
            Date = date;
            TotalSets = totalSets;
        }
    }
}
