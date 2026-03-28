using System;

namespace Sportik.Desktop.Core.Models
{
    public sealed class AddExerciseSetModel
    {
        public Guid? Id { get; }

        public int Repetitions { get; }

        public DateTimeOffset LoggedAt { get; }

        public Guid ExerciseId { get; }

        public AddExerciseSetModel(Guid? id, int repetitions, DateTimeOffset loggedAt, Guid exerciseId)
        {
            Id = id;
            Repetitions = repetitions;
            LoggedAt = loggedAt;
            ExerciseId = exerciseId;
        }
    }
}