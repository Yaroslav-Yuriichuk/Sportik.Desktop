using System;
using Sportik.Core.Models;

namespace Sportik.Automation.Events
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
