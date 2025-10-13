using System;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Events
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
