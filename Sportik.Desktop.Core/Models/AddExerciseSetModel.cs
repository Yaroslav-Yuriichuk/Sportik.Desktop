using System;

namespace Sportik.Desktop.Core.Models
{
    public sealed class AddExerciseSetModel
    {
        public Guid? Id { get; }

        public int Repetitions { get; }

        public DateTimeOffset LoggedAt { get; }

        public AddExerciseSetModel(Guid? id, int repetitions, DateTimeOffset loggedAt)
        {
            Id = id;
            Repetitions = repetitions;
            LoggedAt = loggedAt;
        }
    }
}