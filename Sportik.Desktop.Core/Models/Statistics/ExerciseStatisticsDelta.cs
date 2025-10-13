using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Models.Statistics
{
    public sealed class ExerciseStatisticsDelta
    {
        public Exercise Exercise { get; set; }

        public int Sets { get; set; }

        public int Repetitions { get; set; }
    }
}
