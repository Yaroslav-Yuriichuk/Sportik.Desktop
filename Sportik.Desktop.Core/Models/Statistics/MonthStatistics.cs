using System;
using System.Collections.Generic;

namespace Sportik.Desktop.Core.Models.Statistics
{
    public sealed class MonthStatistics
    {
        public DateTime FirstMonthDayDate { get; }

        public DateTime LastMonthDayDate { get; }

        public List<WeekStatistics> WeekStatistics { get; }

        public MonthStatistics(DateTime firstMonthDayDate, DateTime lastMonthDayDate, List<WeekStatistics> weekStatistics)
        {
            FirstMonthDayDate = firstMonthDayDate;
            LastMonthDayDate = lastMonthDayDate;
            WeekStatistics = weekStatistics;
        }
    }
}
