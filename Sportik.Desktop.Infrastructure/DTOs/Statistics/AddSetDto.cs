using System;

namespace Sportik.Desktop.Infrastructure.DTOs.Statistics
{
    public sealed class AddSetDto
    {
        public Guid ExerciseId { get; }

        public int Repetitions { get; }

        public DateTimeOffset LoggedAt { get; }

        public AddSetDto(Guid exerciseId, int repetitions, DateTimeOffset loggedAt)
        {
            ExerciseId = exerciseId;
            Repetitions = repetitions;
            LoggedAt = loggedAt;
        }
    }
}