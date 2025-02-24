using System;
using Sportik.Core.Models;

namespace Sportik.Automation.Events
{
    public sealed class ExerciseCompleteRequestedEventArgs : EventArgs
    {
        public Exercise Exercise { get; }

        public ExerciseCompleteRequestedEventArgs(Exercise exercise)
        {
            Exercise = exercise;
        }
    }
}
