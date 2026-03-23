using System;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Infrastructure.DTOs.Exercises;

namespace Sportik.Desktop.Infrastructure.Mappers
{
    internal static class ExerciseMapper
    {
        public static Exercise ToDomain(ExerciseDto dto, bool isEnabled)
        {
            return new Exercise(
                id: dto.Id,
                name: dto.Name,
                settings: ExerciseSettingsMapper.ToDomain(dto.Settings, isEnabled));
        }

        public static AddExerciseDto ToDto(AddExerciseModel exercise)
        {
            return new AddExerciseDto(
                exercise.Id,
                exercise.Name,
                ExerciseSettingsMapper.ToDto(exercise.Settings));
        }
    }
}