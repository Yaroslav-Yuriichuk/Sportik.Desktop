using System;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ExerciseSetAddedEventArgs : EventArgs
    {
        public ExerciseSet Set { get; }

        public CreationSource CreationSource { get; }

        public ExerciseSetAddedEventArgs(ExerciseSet set, CreationSource creationSource)
        {
            Set = set;
            CreationSource = creationSource;
        }
    }
}