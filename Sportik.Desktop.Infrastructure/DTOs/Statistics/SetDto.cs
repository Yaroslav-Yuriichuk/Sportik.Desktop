using System;

namespace Sportik.Desktop.Infrastructure.DTOs.Statistics
{
    internal sealed class SetDto
    {
        public Guid Id { get; }

        public int Repetitions { get; }

        public DateTimeOffset LoggedAt { get; }

        public Guid ExerciseId { get; }

        public SetDto(Guid id, int repetitions, DateTimeOffset loggedAt, Guid exerciseId)
        {
            Id = id;
            Repetitions = repetitions;
            LoggedAt = loggedAt;
            ExerciseId = exerciseId;
        }
    }
}