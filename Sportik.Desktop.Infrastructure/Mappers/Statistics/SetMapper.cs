using System;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Infrastructure.DTOs.Statistics;

namespace Sportik.Desktop.Infrastructure.Mappers.Statistics
{
    internal static class SetMapper
    {
        public static ExerciseSet ToDomain(SetDto dto)
        {
            return new ExerciseSet(dto.Repetitions, dto.LoggedAt);
        }

        public static AddSetDto ToDto(ExerciseSet domain, Guid exerciseId)
        {
            return new AddSetDto(exerciseId, domain.Repetitions, domain.LoggedAt);
        }
    }
}