using System;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ExerciseSetAddedEventArgs : EventArgs
    {
        public ExerciseSet Set { get; }

        public ExerciseSetAddedEventArgs(ExerciseSet set)
        {
            Set = set;
        }
    }
}