using System;
using Sportik.UWP.Models;

namespace Sportik.UWP.Services.Settings
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
