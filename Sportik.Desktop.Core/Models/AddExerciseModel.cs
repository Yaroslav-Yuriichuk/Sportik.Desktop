using System;
using Sportik.Desktop.Core.Models.Settings;

namespace Sportik.Desktop.Core.Models
{
    public sealed class AddExerciseModel
    {
        public Guid? Id { get; }

        public string Name { get; }

        public AddExerciseSettingsModel Settings { get; }

        public AddExerciseModel(Guid? id, string name, AddExerciseSettingsModel settings)
        {
            Id = id;
            Name = name.Trim();
            Settings = settings;
        }
    }
}