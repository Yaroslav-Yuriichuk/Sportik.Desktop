namespace Sportik.Desktop.Core.Models.Statistics
{
    public sealed class AggregatedSetsExerciseStatistics
    {
        public Exercise Exercise { get; }

        public int TotalSets { get; }

        public AggregatedSetsExerciseStatistics(Exercise exercise, int totalSets)
        {
            Exercise = exercise;
            TotalSets = totalSets;
        }
    }
}