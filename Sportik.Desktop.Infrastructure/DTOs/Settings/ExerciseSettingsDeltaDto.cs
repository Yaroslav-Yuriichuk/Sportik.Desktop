using System;

namespace Sportik.Desktop.Infrastructure.DTOs.Settings
{
    internal sealed class ExerciseSettingsDeltaDto
    {
        public int? TargetRepetitions { get; }

        public TimeSpan? TimeBetweenSets { get; }

        public TimeSpan? ExecutionTime { get; }

        public ExerciseSettingsDeltaDto(int? targetRepetitions, TimeSpan? timeBetweenSets, TimeSpan? executionTime)
        {
            TargetRepetitions = targetRepetitions;
            TimeBetweenSets = timeBetweenSets;
            ExecutionTime = executionTime;
        }
    }
}