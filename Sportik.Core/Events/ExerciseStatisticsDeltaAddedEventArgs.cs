using System;
using Sportik.Core.Models;

namespace Sportik.Core.Events
{
    public sealed class ExerciseStatisticsDeltaAddedEventArgs : EventArgs
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
