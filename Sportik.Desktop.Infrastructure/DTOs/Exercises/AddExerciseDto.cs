using System;

namespace Sportik.Desktop.Infrastructure.DTOs.Exercises
{
    internal sealed class AddExerciseDto
    {
        public Guid? Id { get; }

        public string Name { get; }

        public AddExerciseSettingsDto Settings { get; }

        public AddExerciseDto(Guid? id, string name, AddExerciseSettingsDto settings)
        {
            Id = id;
            Name = name;
            Settings = settings;
        }
    }
}