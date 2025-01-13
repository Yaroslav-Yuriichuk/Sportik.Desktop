using Sportik.Models.Settings;

namespace Sportik.Models
{
    internal sealed class Exercise
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ExerciseKind Kind { get; set; }

        public ExerciseSettings ExerciseSettings { get; set; }
    }
}
