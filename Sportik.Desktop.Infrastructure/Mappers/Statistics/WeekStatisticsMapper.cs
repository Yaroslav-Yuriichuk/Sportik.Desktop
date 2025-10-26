using System.Linq;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Infrastructure.DTOs.Statistics;

namespace Sportik.Desktop.Infrastructure.Mappers.Statistics
{
    internal static class WeekStatisticsMapper
    {
        public static WeekStatistics ToDomain(WeekStatisticsDto dto)
        {
            return new WeekStatistics(
                dto.DayStatistics.Select(DayStatisticsMapper.ToDomain).ToList());
        }
    }
}