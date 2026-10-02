using System;

namespace Sportik.Desktop.Core.Models.Goal
{
    public sealed class AddExerciseGoalModel
    {
        public int Repetitions { get; }

        public DateTime DayInWeek { get; }

        public Guid ExerciseId { get; }

        public AddExerciseGoalModel(int repetitions, DateTime dayInWeek, Guid exerciseId)
        {
            Repetitions = repetitions;
            DayInWeek = dayInWeek;
            ExerciseId = exerciseId;
        }
    }
}