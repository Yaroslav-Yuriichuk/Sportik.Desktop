using System;
using System.Collections.Generic;

namespace Sportik.Core.Models.Statistics
{
    public sealed class DayStatistics
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public List<ExerciseStatistics> ExerciseStatistics { get; set; } = new List<ExerciseStatistics>();
    }
}
