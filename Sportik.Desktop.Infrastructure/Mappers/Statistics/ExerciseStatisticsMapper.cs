using System.Linq;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Infrastructure.DTOs.Statistics;

namespace Sportik.Desktop.Infrastructure.Mappers.Statistics
{
    internal static class ExerciseStatisticsMapper
    {
        public static ExerciseStatistics ToDomain(ExerciseStatisticsDto dto)
        {
            return new ExerciseStatistics(
                dto.ExerciseId,
                dto.Sets.Select(SetMapper.ToDomain).ToList());
        }
    }
}