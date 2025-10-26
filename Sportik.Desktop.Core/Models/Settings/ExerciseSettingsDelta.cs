using System;

namespace Sportik.Desktop.Core.Models.Settings
{
    [Flags]
    public enum ExerciseSettingsChange
    {
        None = 0,
        IsEnabled = 1 << 0,
        TargetRepetitions = 1 << 1,
        TimeBetweenSets = 1 << 2,
        ExecutionTime = 1 << 3,
    }

    public sealed class ExerciseSettingsDelta
    {
        public ExerciseSettingsChange Change { get; set; }

        public bool IsEnabled { get; set; }

        public int TargetRepetitions { get; set; }

        public TimeSpan TimeBetweenSets { get; set; }

        public TimeSpan ExecutionTime { get; set; }
    }
}
