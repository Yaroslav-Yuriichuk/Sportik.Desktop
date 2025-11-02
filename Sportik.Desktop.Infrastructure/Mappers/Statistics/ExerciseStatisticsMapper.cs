using System;
using System.Linq;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Infrastructure.DTOs.Statistics;

namespace Sportik.Desktop.Infrastructure.Mappers.Statistics
{
    internal static class ExerciseStatisticsMapper
    {
        public static ExerciseStatistics ToDomain(ExerciseStatisticsDto dto, Func<Guid, bool> isExerciseEnabled)
        {
            return new ExerciseStatistics(
                ExerciseMapper.ToDomain(dto.Exercise, isExerciseEnabled(dto.Exercise.Id)),
                dto.Sets.Select(SetMapper.ToDomain).ToList());
        }
    }
}