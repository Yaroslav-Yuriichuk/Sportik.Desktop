using System;
using System.Collections.Generic;

namespace Sportik.Desktop.Core.Models.Statistics
{
    public sealed class DayStatistics
    {
        public DateTime Date { get; }

        public IReadOnlyList<ExerciseStatistics> ExerciseStatistics { get; }

        public DayStatistics(DateTime date, IReadOnlyList<ExerciseStatistics> exerciseStatistics)
        {
            Date = date;
            ExerciseStatistics = exerciseStatistics;
        }
    }
}
