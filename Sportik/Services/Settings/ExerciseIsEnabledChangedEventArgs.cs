using System;
using Sportik.Models;

namespace Sportik.Services.Settings
{
    internal class ExerciseIsEnabledChangedEventArgs : EventArgs
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
