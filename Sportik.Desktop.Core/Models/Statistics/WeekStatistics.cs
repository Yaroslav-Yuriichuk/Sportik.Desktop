using System;
using System.Collections.Generic;

namespace Sportik.Desktop.Core.Models.Statistics
{
    public sealed class WeekStatistics
    {
        public DateTime FirstWeekDayDate { get; }

        public DateTime LastWeekDayDate { get; }

        public IReadOnlyList<DayStatistics> DayStatistics { get; }

        public WeekStatistics(DateTime firstWeekDayDate, DateTime lastWeekDayDate, IReadOnlyList<DayStatistics> dayStatistics)
        {
            FirstWeekDayDate = firstWeekDayDate;
            LastWeekDayDate = lastWeekDayDate;
            DayStatistics = dayStatistics;
        }
    }
}
