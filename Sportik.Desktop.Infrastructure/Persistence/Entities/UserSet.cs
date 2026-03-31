using System;

namespace Sportik.Desktop.Infrastructure.Persistence.Entities
{
    internal sealed class UserSet
    {
        public Guid Id { get; private set; }

        public int Repetitions { get; private set; }

        public DateTimeOffset LoggedAt { get; private set; }

        public Guid ExerciseId { get; private set; }

        public UserExercise Exercise { get; private set; } = null!;

        public UserSet(Guid id, int repetitions, DateTimeOffset loggedAt, Guid exerciseId)
        {
            Id = id;
            Repetitions = repetitions;
            LoggedAt = loggedAt;
            ExerciseId = exerciseId;
        }

        public UserSet(Guid id, int repetitions, DateTimeOffset loggedAt, UserExercise exercise)
        {
            Id = id;
            Repetitions = repetitions;
            LoggedAt = loggedAt;
            ExerciseId = exercise.Id;
            Exercise = exercise;
        }
    }
}