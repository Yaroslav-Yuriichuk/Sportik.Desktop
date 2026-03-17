using System;

namespace Sportik.Desktop.Infrastructure.Persistence.Entities
{
    internal sealed class UserExercise
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public Guid SettingsId { get; private set; }

        public UserExerciseSettings Settings { get; private set; } = null!;

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