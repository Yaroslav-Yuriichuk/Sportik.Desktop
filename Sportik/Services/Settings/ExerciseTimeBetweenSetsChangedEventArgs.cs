using System;
using Sportik.Models;

namespace Sportik.Services.Settings
{
    internal sealed class ExerciseTimeBetweenSetsChangedEventArgs : EventArgs
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
