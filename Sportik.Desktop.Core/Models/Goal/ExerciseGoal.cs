using System;

namespace Sportik.Desktop.Core.Models.Goal
{
    public sealed class ExerciseGoal
    {
        public Exercise Exercise { get; }

        public int TargetRepetitions { get; }

        public int CompletedRepetitions { get; }

        public ExerciseGoal(Exercise exercise, int targetRepetitions, int completedRepetitions)
        {
            Exercise = exercise ?? throw new ArgumentNullException(nameof(exercise));
            TargetRepetitions = targetRepetitions;
            CompletedRepetitions = completedRepetitions;
        }
    }
}