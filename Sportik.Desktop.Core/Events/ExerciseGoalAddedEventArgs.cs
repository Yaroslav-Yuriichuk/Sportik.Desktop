using System;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Goal;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ExerciseGoalAddedEventArgs : EventArgs
    {
        public ExerciseGoal ExerciseGoal { get; }

        public CreationSource CreationSource { get; }

        public ExerciseGoalAddedEventArgs(ExerciseGoal exerciseGoal, CreationSource creationSource)
        {
            ExerciseGoal = exerciseGoal;
            CreationSource = creationSource;
        }
    }
}