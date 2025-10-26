using System;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Infrastructure.DTOs.Settings;

namespace Sportik.Desktop.Infrastructure.Mappers
{
    internal static class ExerciseSettingsDeltaMapper
    {
        public static ExerciseSettingsDeltaDto ToDto(ExerciseSettingsDelta domain)
        {
            ExerciseSettingsChange change = domain.Change;

            int? targetRepetitions = change.HasFlag(ExerciseSettingsChange.TargetRepetitions)
                ? domain.TargetRepetitions
                : (int?)null;

            TimeSpan? timeBetweenSets = change.HasFlag(ExerciseSettingsChange.TimeBetweenSets)
                ? domain.TimeBetweenSets
                : (TimeSpan?)null;

            TimeSpan? executionTime = change.HasFlag(ExerciseSettingsChange.ExecutionTime)
                ? domain.ExecutionTime
                : (TimeSpan?)null;

            return new ExerciseSettingsDeltaDto(targetRepetitions, timeBetweenSets, executionTime);
        }
    }
}