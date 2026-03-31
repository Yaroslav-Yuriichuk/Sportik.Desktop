using System;

namespace Sportik.Desktop.Core.Models
{
    [Flags]
    public enum SyncOption
    {
        None = 0,
        Exercises = 1 << 0,
        ExerciseSettings = 1 << 1,
        ExerciseStatistics = 1 << 2,
        All = Exercises | ExerciseSettings | ExerciseStatistics,
    }
}