using System;
using System.Collections.Generic;

namespace Sportik.Desktop.Core.Models.Statistics
{
    public sealed class YearStatistics
    {
        public DateTime FirstYearDate { get; }

        public DateTime LastYearDate { get; }

        public IReadOnlyList<MonthStatistics> MonthStatistics { get; }

        public YearStatistics(DateTime firstYearDate, DateTime lastYearDate, IReadOnlyList<MonthStatistics> monthStatistics)
        {
            FirstYearDate = firstYearDate;
            LastYearDate = lastYearDate;
            MonthStatistics = monthStatistics;
        }
    }
}