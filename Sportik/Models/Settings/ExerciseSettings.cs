using System;

namespace Sportik.Models.Settings
{
    internal sealed class ExerciseSettings
    {
        public int Id { get; set; }

        public int ExerciseId { get; set; }

        public bool IsEnabled { get; set; }

        public int TargetRepetitions { get; set; }

        public TimeSpan TimeBetweenSets { get; set; }

        public TimeSpan ExecutionTime { get; set; }

        public Exercise Exercise { get; set; }
    }
}
