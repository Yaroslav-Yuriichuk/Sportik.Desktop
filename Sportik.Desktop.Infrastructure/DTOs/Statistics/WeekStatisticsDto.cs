using System.Collections.Generic;

namespace Sportik.Desktop.Infrastructure.DTOs.Statistics
{
    internal sealed class WeekStatisticsDto
    {
        public List<DayStatisticsDto> DayStatistics { get; }

        public WeekStatisticsDto(List<DayStatisticsDto> dayStatistics)
        {
            DayStatistics = dayStatistics;
        }
    }
}