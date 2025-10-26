using System;
using System.Collections.Generic;

namespace Sportik.Desktop.Infrastructure.DTOs.Statistics
{
    internal sealed class DayStatisticsDto
    {
        public DateTime Date { get; }

        public List<ExerciseStatisticsDto> ExerciseStatistics { get; }

        public DayStatisticsDto(DateTime date, List<ExerciseStatisticsDto> exerciseStatistics)
        {
            Date = date;
            ExerciseStatistics = exerciseStatistics;
        }
    }
}