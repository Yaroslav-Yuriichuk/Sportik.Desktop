using System;

namespace Sportik.Desktop.Infrastructure.DTOs
{
    internal sealed class ExerciseSettingsDto
    {
        public int TargetRepetitions { get; }

        public TimeSpan TimeBetweenSets { get; }

        public TimeSpan ExecutionTime { get; }

        public ExerciseSettingsDto(int targetRepetitions, TimeSpan timeBetweenSets, TimeSpan executionTime)
        {
            TargetRepetitions = targetRepetitions;
            TimeBetweenSets = timeBetweenSets;
            ExecutionTime = executionTime;
        }
    }
}