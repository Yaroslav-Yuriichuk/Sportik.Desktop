namespace Sportik.Desktop.Core.Models.Statistics
{
    public sealed class AggregatedSetsMonthStatistics
    {
        public int Month { get; }

        public int TotalSets { get; }

        public AggregatedSetsMonthStatistics(int month, int totalSets)
        {
            Month = month;
            TotalSets = totalSets;
        }
    }
}