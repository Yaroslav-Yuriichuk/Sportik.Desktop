using System;

namespace Sportik.Desktop.Core.Models.Settings
{
    public sealed class ExerciseSettings
    {
        public bool IsEnabled { get; }

        public int TargetRepetitions { get; }

        public TimeSpan TimeBetweenSets { get; }

        public TimeSpan ExecutionTime { get; }

        public ExerciseSettings(bool isEnabled, int targetRepetitions, TimeSpan timeBetweenSets, TimeSpan executionTime)
        {
            if (targetRepetitions < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(targetRepetitions), "Target repetitions must be at least 1.");
            }

            if (timeBetweenSets < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeBetweenSets), "Time between sets cannot be negative.");
            }

            if (executionTime < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(executionTime), "Execution time cannot be negative.");
            }

            IsEnabled = isEnabled;
            TargetRepetitions = targetRepetitions;
            TimeBetweenSets = timeBetweenSets;
            ExecutionTime = executionTime;
        }
    }
}
