using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.UI.ViewModels.Training
{
    internal sealed class SetupSetViewModel : ViewModel
    {
        private Exercise _exercise;

        public Exercise Exercise
        {
            get => _exercise;
            set => SetField(ref _exercise, value);
        }

        private int _repetitions;

        public int Repetitions
        {
            get => _repetitions;
            set => SetField(ref _repetitions, value);
        }

        public SetupSetViewModel(Exercise exercise, int repetitions)
        {
            Exercise = exercise;
            Repetitions = repetitions;
        }
    }
}