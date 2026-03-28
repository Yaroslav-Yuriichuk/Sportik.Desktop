using System;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Infrastructure.Persistence.Entities;

namespace Sportik.Desktop.Infrastructure.Persistence.Mappers
{
    internal static class ExerciseMapper
    {
        public static Exercise ToDomain(UserExercise entity, bool isEnabled)
        {
            return new Exercise(
                entity.Id,
                entity.Name,
                ExerciseSettingsMapper.ToDomain(entity.Settings, isEnabled));
        }

        public static UserExercise ToEntity(AddExerciseModel exercise)
        {
            return new UserExercise(
                exercise.Id ?? Guid.NewGuid(),
                exercise.Name,
                ExerciseSettingsMapper.ToEntity(exercise.Settings));
        }
    }
}