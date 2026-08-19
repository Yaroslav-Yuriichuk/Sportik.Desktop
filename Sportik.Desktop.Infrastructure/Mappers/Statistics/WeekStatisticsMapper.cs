using System;
using System.Linq;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Infrastructure.DTOs.Statistics;

namespace Sportik.Desktop.Infrastructure.Mappers.Statistics
{
    internal static class WeekStatisticsMapper
    {
        public static WeekStatistics ToDomain(WeekStatisticsDto dto, Func<Guid, bool> isExerciseEnabled)
        {
            DayStatisticsDto dayStatistics = dto.DayStatistics.First();

            DateTime firstDayOfWeek = CalendarHelper.GetFirstDayOfWeek(dayStatistics.Date);
            DateTime lastDayOfWeek = CalendarHelper.GetLastDayOfWeek(dayStatistics.Date);

            return new WeekStatistics(
                firstDayOfWeek,
                lastDayOfWeek,
                dto.DayStatistics.Select(ds => DayStatisticsMapper.ToDomain(ds, isExerciseEnabled)).ToList());
        }
    }
}