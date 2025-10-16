using System;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ExerciseTimeBetweenSetsChangedEventArgs : EventArgs
    {
        public Guid ExerciseId { get; }

        public TimeSpan TimeBetweenSets { get; }

        public ExerciseTimeBetweenSetsChangedEventArgs(Guid exerciseId, TimeSpan timeBetweenSets)
        {
            ExerciseId = exerciseId;
            TimeBetweenSets = timeBetweenSets;
        }
    }
}
