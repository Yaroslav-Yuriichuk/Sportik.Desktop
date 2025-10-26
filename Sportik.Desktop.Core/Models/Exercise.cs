using System;
using Sportik.Desktop.Core.Models.Settings;

namespace Sportik.Desktop.Core.Models
{
    public sealed class Exercise
    {
        public Guid Id { get; }

        public string Name { get; }

        public ExerciseSettings Settings { get; }

        public Exercise(Guid id, string name, ExerciseSettings settings)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }
    }
}
