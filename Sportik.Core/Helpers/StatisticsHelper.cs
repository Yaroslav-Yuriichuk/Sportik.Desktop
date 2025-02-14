using System;
using System.Linq;
using Sportik.Core.Models.Statistics;

namespace Sportik.Core.Helpers
{
    public sealed class StatisticsHelper
    {
        public static DateTime GetWeekFirstDayDate(WeekStatistics weekStatistics)
        {
            DateTime firstDayDate = GetFirstDayDate(weekStatistics);
            return CalendarHelper.GetFirstDayOfWeek(firstDayDate);
        }

        public static DateTime GetWeekLastDayDate(WeekStatistics weekStatistics)
        {
            DateTime lastDayDate = GetLastDayDate(weekStatistics);
            return CalendarHelper.GetLastDayOfWeek(lastDayDate);
        }

        public static DateTime GetFirstDayDate(WeekStatistics weekStatistics)
        {
            DayStatistics dayStatistics = weekStatistics.DayStatistics
                .OrderBy(statistics => statistics.Date)
                .First();

            return dayStatistics.Date;
        }

        public static DateTime GetLastDayDate(WeekStatistics weekStatistics)
        {
            DayStatistics dayStatistics = weekStatistics.DayStatistics
                .OrderBy(statistics => statistics.Date)
                .Last();

            return dayStatistics.Date;
        }
    }
}
