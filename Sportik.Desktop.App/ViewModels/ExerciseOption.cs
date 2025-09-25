using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.App.ViewModels
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
