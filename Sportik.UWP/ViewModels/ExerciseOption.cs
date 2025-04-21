using Sportik.Core.Models;

namespace Sportik.UWP.ViewModels
{
    internal sealed class ExerciseOption
    {
        public Exercise Exercise { get; set; }

        public ExerciseOption(Exercise exercise)
        {
            Exercise = exercise;
        }
    }
}
