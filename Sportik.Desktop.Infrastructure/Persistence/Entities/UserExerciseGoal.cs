using System;

namespace Sportik.Desktop.Infrastructure.Persistence.Entities
{
    internal sealed class UserExerciseGoal
    {
        public Guid Id { get; private set; }

        public int Repetitions { get; set; }

        public DateTime FirstWeekDayDate { get; private set; }

        public Guid ExerciseId { get; private set; }

        public UserExercise Exercise { get; private set; }

        public UserExerciseGoal(Guid id, int repetitions, DateTime firstWeekDayDate, Guid exerciseId)
            : this(id, repetitions, firstWeekDayDate, exerciseId, null)
        {
        }

        public UserExerciseGoal(Guid id, int repetitions, DateTime firstWeekDayDate, UserExercise exercise)
            : this(id, repetitions, firstWeekDayDate, exercise.Id, exercise)
        {
        }

        private UserExerciseGoal(Guid id, int repetitions, DateTime firstWeekDayDate, Guid exerciseId, UserExercise exercise)
        {
            if (exercise != null && exerciseId != exercise.Id)
            {
                throw new ArgumentException("ExerciseId does not match the Id of the provided exercise.", nameof(exerciseId));
            }

            Id = id;
            Repetitions = repetitions;
            FirstWeekDayDate = firstWeekDayDate;
            ExerciseId = exerciseId;
            Exercise = exercise;
        }
    }
}