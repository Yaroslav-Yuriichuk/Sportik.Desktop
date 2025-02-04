using Sportik.Core.Models;

namespace Sportik.Core.Models.Statistics
{
    public sealed class ExerciseStatisticsDelta
    {
        public Exercise Exercise { get; set; }

        public int Sets { get; set; }

        public int Repetitions { get; set; }
    }
}
