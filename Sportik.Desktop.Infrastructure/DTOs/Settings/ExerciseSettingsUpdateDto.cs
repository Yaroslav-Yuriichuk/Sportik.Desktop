using System;

namespace Sportik.Desktop.Infrastructure.DTOs.Settings
{
    internal sealed class ExerciseSettingsUpdateDto
    {
        public ExerciseSettingsDeltaDto Delta { get; }

        public Guid ExerciseId { get; }

        public ExerciseSettingsUpdateDto(ExerciseSettingsDeltaDto delta, Guid exerciseId)
        {
            Delta = delta;
            ExerciseId = exerciseId;
        }
    }
}