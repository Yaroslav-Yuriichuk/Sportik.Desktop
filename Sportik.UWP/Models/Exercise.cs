using Sportik.UWP.Models.Settings;

namespace Sportik.UWP.Models
{
    internal sealed class Exercise
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ExerciseKind Kind { get; set; }

        public ExerciseSettings ExerciseSettings { get; set; }
    }
}
