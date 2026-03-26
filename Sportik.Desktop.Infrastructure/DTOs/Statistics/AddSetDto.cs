using System;

namespace Sportik.Desktop.Infrastructure.DTOs.Statistics
{
    public sealed class AddSetDto
    {
        public Guid? Id { get; }

        public Guid ExerciseId { get; }

        public int Repetitions { get; }

        public DateTimeOffset LoggedAt { get; }

        public AddSetDto(Guid? id, Guid exerciseId, int repetitions, DateTimeOffset loggedAt)
        {
            Id = id;
            ExerciseId = exerciseId;
            Repetitions = repetitions;
            LoggedAt = loggedAt;
        }
    }
}