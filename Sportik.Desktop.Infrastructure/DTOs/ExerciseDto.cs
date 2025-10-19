using System;

namespace Sportik.Desktop.Infrastructure.DTOs
{
    internal sealed class ExerciseDto
    {
        public Guid Id { get; }

        public string Name { get; }

        public ExerciseSettingsDto Settings { get; }

        public ExerciseDto(Guid id, string name, ExerciseSettingsDto settings)
        {
            Id = id;
            Name = name;
            Settings = settings;
        }
    }
}