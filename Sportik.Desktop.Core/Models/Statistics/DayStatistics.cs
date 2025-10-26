using System;
using System.Collections.Generic;

namespace Sportik.Desktop.Core.Models.Statistics
{
    public sealed class DayStatistics
    {
        public DateTime Date { get; }

        public List<ExerciseStatistics> ExerciseStatistics { get; }

        public DayStatistics(DateTime date, List<ExerciseStatistics> exerciseStatistics)
        {
            Date = date;
            ExerciseStatistics = exerciseStatistics;
        }
    }
}
