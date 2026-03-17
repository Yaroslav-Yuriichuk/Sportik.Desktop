using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Infrastructure.Persistence.Entities;

namespace Sportik.Desktop.Infrastructure.Persistence.Mappers
{
    internal static class ExerciseSettingsMapper
    {
        public static ExerciseSettings ToDomain(UserExerciseSettings entity, bool isEnabled)
        {
            return new ExerciseSettings(
                isEnabled,
                entity.TargetRepetitions,
                entity.TimeBetweenSets,
                entity.ExecutionTime);
        }

        public static UserExerciseSettings ToEntity(ExerciseSettings settings)
        {
            return new UserExerciseSettings
            {
                TargetRepetitions = settings.TargetRepetitions,
                TimeBetweenSets = settings.TimeBetweenSets,
                ExecutionTime = settings.ExecutionTime
            };
        }
    }
}