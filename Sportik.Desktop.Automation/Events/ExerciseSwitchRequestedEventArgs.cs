using System;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Automation.Events
{
    public sealed class ExerciseSwitchRequestedEventArgs : EventArgs
    {
        public Exercise Exercise { get; }

        public ExerciseSwitchRequestedEventArgs(Exercise exercise)
        {
            Exercise = exercise;
        }
    }
}
