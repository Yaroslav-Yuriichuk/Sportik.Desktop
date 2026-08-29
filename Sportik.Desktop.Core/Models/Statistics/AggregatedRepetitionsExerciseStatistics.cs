namespace Sportik.Desktop.Core.Models.Statistics
{
    public sealed class AggregatedRepetitionsExerciseStatistics
    {
        public Exercise Exercise { get; }

        public int TotalRepetitions { get; }

        public AggregatedRepetitionsExerciseStatistics(Exercise exercise, int totalRepetitions)
        {
            Exercise = exercise;
            TotalRepetitions = totalRepetitions;
        }
    }
}