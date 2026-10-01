using System;

namespace Sportik.Desktop.Core.Models.Goal
{
    public sealed class ExerciseGoal
    {
        public Exercise Exercise { get; }

        public int Repetitions { get; }

        public ExerciseGoal(Exercise exercise, int repetitions)
        {
            Exercise = exercise ?? throw new ArgumentNullException(nameof(exercise));
            Repetitions = repetitions;
        }
    }
}