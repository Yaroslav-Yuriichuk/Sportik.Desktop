using System;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ExerciseCreatedEventArgs : EventArgs
    {
        public Exercise Exercise { get; }

        internal bool SynchronizationRequired { get; }

        public ExerciseCreatedEventArgs(Exercise exercise, bool synchronizationRequired)
        {
            Exercise = exercise;
            SynchronizationRequired = synchronizationRequired;
        }
    }
}