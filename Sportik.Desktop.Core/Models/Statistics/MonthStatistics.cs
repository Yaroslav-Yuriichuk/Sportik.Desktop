using System;
using System.Collections.Generic;

namespace Sportik.Desktop.Core.Models.Statistics
{
    public sealed class MonthStatistics
    {
        public DateTime FirstMonthDayDate { get; }

        public DateTime LastMonthDayDate { get; }

        public IReadOnlyList<WeekStatistics> WeekStatistics { get; }

        public MonthStatistics(DateTime firstMonthDayDate, DateTime lastMonthDayDate, IReadOnlyList<WeekStatistics> weekStatistics)
        {
            FirstMonthDayDate = firstMonthDayDate;
            LastMonthDayDate = lastMonthDayDate;
            WeekStatistics = weekStatistics;
        }
    }
}
