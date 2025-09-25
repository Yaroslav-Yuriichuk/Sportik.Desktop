using Sportik.Desktop.Core.Models.Settings;

namespace Sportik.Desktop.Core.Models
{
    public sealed class Exercise
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ExerciseKind Kind { get; set; }

        public ExerciseSettings ExerciseSettings { get; set; }
    }
}
