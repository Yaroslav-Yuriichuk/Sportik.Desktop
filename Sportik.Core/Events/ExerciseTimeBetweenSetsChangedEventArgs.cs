using System;
using Sportik.Core.Models;

namespace Sportik.Core.Events
{
    public sealed class ExerciseTimeBetweenSetsChangedEventArgs : EventArgs
    {
        public Exercise Exercise { get; }

        public TimeSpan TimeBetweenSets { get; }

        public ExerciseTimeBetweenSetsChangedEventArgs(Exercise exercise, TimeSpan timeBetweenSets)
        {
            Exercise = exercise;
            TimeBetweenSets = timeBetweenSets;
        }
    }
}
