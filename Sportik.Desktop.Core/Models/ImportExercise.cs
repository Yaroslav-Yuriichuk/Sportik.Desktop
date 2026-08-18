using System;

namespace Sportik.Desktop.Core.Models
{
    public sealed class ImportExercise
    {
        public string Name { get; }

        public DateTimeOffset LoggedAt { get; }

        public int Repetitions { get; }

        public ImportExercise(string name, DateTimeOffset loggedAt, int repetitions)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            LoggedAt = loggedAt;
            Repetitions = repetitions;
        }
    }
}