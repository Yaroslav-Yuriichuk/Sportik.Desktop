using System;
using System.Linq;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Infrastructure.DTOs.Statistics;

namespace Sportik.Desktop.Infrastructure.Mappers.Statistics
{
    internal static class DayStatisticsMapper
    {
        public static DayStatistics ToDomain(DayStatisticsDto dto, Func<Guid, bool> isExerciseEnabled)
        {
            return new DayStatistics(
                dto.Date,
                dto.ExerciseStatistics.Select(es => ExerciseStatisticsMapper.ToDomain(es, isExerciseEnabled)).ToList());
        }
    }
}