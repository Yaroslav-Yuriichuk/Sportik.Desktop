using System;
using Sportik.Core.Models;

namespace Sportik.Core.Events
{
    public class ExerciseIsEnabledChangedEventArgs : EventArgs
    {
        public Exercise Exercise { get; }

        public bool IsEnabled { get; }

        public ExerciseIsEnabledChangedEventArgs(Exercise exercise, bool isEnabled)
        {
            Exercise = exercise;
            IsEnabled = isEnabled;
        }
    }
}
