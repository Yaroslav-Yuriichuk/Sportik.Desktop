using System;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ExerciseSetAddedEventArgs : EventArgs
    {
        public ExerciseSet Set { get; }

        internal bool SynchronizationRequired { get; }

        public ExerciseSetAddedEventArgs(ExerciseSet set, bool synchronizationRequired)
        {
            Set = set;
            SynchronizationRequired = synchronizationRequired;
        }
    }
}