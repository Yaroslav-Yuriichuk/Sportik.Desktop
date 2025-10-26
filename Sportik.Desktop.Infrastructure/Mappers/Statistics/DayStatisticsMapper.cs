using System.Linq;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Infrastructure.DTOs.Statistics;

namespace Sportik.Desktop.Infrastructure.Mappers.Statistics
{
    internal static class DayStatisticsMapper
    {
        public static DayStatistics ToDomain(DayStatisticsDto dto)
        {
            return new DayStatistics(
                dto.Date,
                dto.ExerciseStatistics.Select(ExerciseStatisticsMapper.ToDomain).ToList());
        }
    }
}