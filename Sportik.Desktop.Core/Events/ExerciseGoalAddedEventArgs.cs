using System;
using Sportik.Desktop.Core.Models.Goal;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ExerciseGoalAddedEventArgs : EventArgs
    {
        public ExerciseGoal ExerciseGoal { get; }

        public ExerciseGoalAddedEventArgs(ExerciseGoal exerciseGoal)
        {
            ExerciseGoal = exerciseGoal;
        }
    }
}