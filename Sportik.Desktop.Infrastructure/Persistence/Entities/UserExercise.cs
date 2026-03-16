using System;

namespace Sportik.Desktop.Infrastructure.Persistence.Entities
{
    internal sealed class UserExercise
    {
        public Guid Id { get; }

        public string Name { get; }

        public Guid SettingsId { get; }

        public UserExerciseSettings Settings { get; } = null!;

        public UserExercise(Guid id, string name, Guid settingsId)
        {
            Id = id;
            Name = name;
            SettingsId = settingsId;
        }

        public UserExercise(Guid id, string name, UserExerciseSettings settings)
        {
            Id = id;
            Name = name;
            SettingsId = settings.Id;
            Settings = settings;
        }
    }
}