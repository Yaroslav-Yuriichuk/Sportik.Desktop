using System;

namespace Sportik.Desktop.Infrastructure.DTOs.Exercises
{
    internal sealed class AddExerciseSettingsDto
    {
        public int TargetRepetitions { get; }

        public TimeSpan TimeBetweenSets { get; }

        public TimeSpan ExecutionTime { get; }

        public AddExerciseSettingsDto(int targetRepetitions, TimeSpan timeBetweenSets, TimeSpan executionTime)
        {
            TargetRepetitions = targetRepetitions;
            TimeBetweenSets = timeBetweenSets;
            ExecutionTime = executionTime;
        }
    }
}