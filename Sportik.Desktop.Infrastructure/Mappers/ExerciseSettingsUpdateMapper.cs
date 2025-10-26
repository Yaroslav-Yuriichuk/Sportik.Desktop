using System;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Infrastructure.DTOs.Settings;

namespace Sportik.Desktop.Infrastructure.Mappers
{
    internal static class ExerciseSettingsUpdateMapper
    {
        public static ExerciseSettingsUpdateDto ToDto(ExerciseSettingsDelta delta, Guid exerciseId)
        {
            return new ExerciseSettingsUpdateDto(ExerciseSettingsDeltaMapper.ToDto(delta), exerciseId);
        }
    }
}