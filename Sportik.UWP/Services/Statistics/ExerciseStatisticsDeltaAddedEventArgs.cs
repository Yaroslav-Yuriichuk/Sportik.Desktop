using System;
using Sportik.UWP.Models;

namespace Sportik.UWP.Services.Statistics
{
    internal sealed class ExerciseStatisticsDeltaAddedEventArgs : EventArgs
    {
        public Exercise Exercise { get; }

        public int SetsDelta { get; }

        public int RepetitionsDelta { get; }

        public ExerciseStatisticsDeltaAddedEventArgs(Exercise exercise, int setsDelta, int repetitionsDelta)
        {
            Exercise = exercise;
            SetsDelta = setsDelta;
            RepetitionsDelta = repetitionsDelta;
        }
    }
}
