using System.Collections.Generic;

namespace Sportik.Desktop.Core.Models.Statistics
{
    public sealed class ExerciseStatistics
    {
        public Exercise Exercise { get; }

        public IReadOnlyList<ExerciseSet> Sets { get; }

        public ExerciseStatistics(Exercise exercise, IReadOnlyList<ExerciseSet> sets)
        {
            Exercise = exercise;
            Sets = sets;
        }
    }
}
