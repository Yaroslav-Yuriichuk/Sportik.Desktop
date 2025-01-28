using System;
using Sportik.UWP.Models;

namespace Sportik.UWP.Services.Settings
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
