using System;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ExerciseCreatedEventArgs : EventArgs
    {
        public Exercise Exercise { get; }

        public ExerciseCreatedEventArgs(Exercise exercise)
        {
            Exercise = exercise;
        }
    }
}