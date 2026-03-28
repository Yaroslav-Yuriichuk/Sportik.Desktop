using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Infrastructure.DTOs.Settings;

namespace Sportik.Desktop.Infrastructure.Mappers
{
    internal static class ExerciseSettingsUpdateMapper
    {
        public static ExerciseSettingsUpdateDto ToDto(UpdateExerciseSettingsModel updateModel)
        {
            return new ExerciseSettingsUpdateDto(
                ExerciseSettingsDeltaMapper.ToDto(updateModel.Delta),
                updateModel.ExerciseId);
        }
    }
}