using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Infrastructure.DTOs.Exercises;

namespace Sportik.Desktop.Infrastructure.Mappers
{
    internal static class ExerciseSettingsMapper
    {
        public static ExerciseSettings ToDomain(ExerciseSettingsDto dto, bool isEnabled)
        {
            return new ExerciseSettings(
                isEnabled,
                dto.TargetRepetitions,
                dto.TimeBetweenSets,
                dto.ExecutionTime);
        }

        public static AddExerciseSettingsDto ToDto(ExerciseSettings settings)
        {
            return new AddExerciseSettingsDto(
                settings.TargetRepetitions,
                settings.TimeBetweenSets,
                settings.ExecutionTime);
        }
    }
}