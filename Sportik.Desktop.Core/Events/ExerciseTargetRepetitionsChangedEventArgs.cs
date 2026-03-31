using System;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ExerciseTargetRepetitionsChangedEventArgs : EventArgs
    {
        public Guid ExerciseId { get; }

        public int NewTargetRepetitions { get; }

        public ExerciseTargetRepetitionsChangedEventArgs(Guid exerciseId, int newTargetRepetitions)
        {
            ExerciseId = exerciseId;
            NewTargetRepetitions = newTargetRepetitions;
        }
    }
}