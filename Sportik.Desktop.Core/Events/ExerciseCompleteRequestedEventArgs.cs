using System;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Events
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
