using System;
using Sportik.Desktop.Core.Models.Settings;

namespace Sportik.Desktop.Core.Models
{
    public sealed class UpdateExerciseSettingsModel
    {
        public Guid ExerciseId { get; }

        public ExerciseSettingsDelta Delta { get; }

        public UpdateExerciseSettingsModel(Guid exerciseId, ExerciseSettingsDelta delta)
        {
            ExerciseId = exerciseId;
            Delta = delta;
        }
    }
}