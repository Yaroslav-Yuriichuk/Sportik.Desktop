using System;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ExerciseTargetRepetitionsChangedEventArgs : EventArgs
    {
        public Guid ExerciseId { get; }

        public int TargetRepetitions { get; }

        public ExerciseTargetRepetitionsChangedEventArgs(Guid exerciseId, int targetRepetitions)
        {
            ExerciseId = exerciseId;
            TargetRepetitions = targetRepetitions;
        }
    }
}