using System;

namespace Sportik.Desktop.Core.Models
{
    public sealed class ExerciseSet
    {
        public int Repetitions { get; }

        public DateTimeOffset LoggedAt { get; }

        public ExerciseSet(int repetitions, DateTimeOffset loggedAt)
        {
            Repetitions = repetitions;
            LoggedAt = loggedAt;
        }
    }
}