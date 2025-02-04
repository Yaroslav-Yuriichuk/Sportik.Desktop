using Sportik.Core.Models;

namespace Sportik.Core.Models.Statistics
{
    public sealed class ExerciseStatistics
    {
        public int Id { get; set; }

        public int ExerciseId { get; set; }

        public int DayStatisticsId { get; set; }

        public int Sets { get; set; }

        public int Repetitions { get; set; }

        public Exercise Exercise { get; set; }

        public DayStatistics DayStatistics { get; set; }
    }
}
