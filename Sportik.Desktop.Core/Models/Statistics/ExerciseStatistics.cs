using System.Collections.Generic;

namespace Sportik.Desktop.Core.Models.Statistics
{
    public sealed class ExerciseStatistics
    {
        public Exercise Exercise { get; }

        public List<ExerciseSet> Sets { get; }

        public ExerciseStatistics(Exercise exercise, List<ExerciseSet> sets)
        {
            Exercise = exercise;
            Sets = sets;
        }
    }
}
