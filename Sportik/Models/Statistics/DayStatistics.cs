using System;
using System.Collections.Generic;

namespace Sportik.Models.Statistics
{
    internal sealed class DayStatistics
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public List<ExerciseStatistics> ExerciseStatistics { get; set; } = new List<ExerciseStatistics>();
    }
}
