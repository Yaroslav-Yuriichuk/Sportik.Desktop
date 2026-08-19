using System;
using System.Collections.Generic;

namespace Sportik.Desktop.Core.Models.Statistics
{
    public sealed class WeekStatistics
    {
        public DateTime FirstWeekDayDate { get; }

        public DateTime LastWeekDayDate { get; }

        public List<DayStatistics> DayStatistics { get; }

        public WeekStatistics(DateTime firstWeekDayDate, DateTime lastWeekDayDate, List<DayStatistics> dayStatistics)
        {
            FirstWeekDayDate = firstWeekDayDate;
            LastWeekDayDate = lastWeekDayDate;
            DayStatistics = dayStatistics;
        }
    }
}
