namespace Sportik.Desktop.Infrastructure.DTOs.Exercises
{
    internal sealed class AddExerciseDto
    {
        public string Name { get; }

        public AddExerciseSettingsDto Settings { get; }

        public AddExerciseDto(string name, AddExerciseSettingsDto settings)
        {
            Name = name;
            Settings = settings;
        }
    }
}