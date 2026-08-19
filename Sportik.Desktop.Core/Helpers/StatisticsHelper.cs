using System;
using System.Collections.Generic;
using System.Linq;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Statistics;

namespace Sportik.Desktop.Core.Helpers
{
    public sealed class StatisticsHelper
    {
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

        public static IEnumerable<ExerciseSet> GetAllSets(IEnumerable<WeekStatistics> weekStatistics)
        {
            return weekStatistics
                .SelectMany(statistics => statistics.DayStatistics)
                .SelectMany(statistics => statistics.ExerciseStatistics)
                .SelectMany(statistics => statistics.Sets);
        }
    }
}
