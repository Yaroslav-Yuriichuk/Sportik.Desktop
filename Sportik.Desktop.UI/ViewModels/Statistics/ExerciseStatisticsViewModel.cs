namespace Sportik.Desktop.UI.ViewModels.Statistics
{
    internal sealed class ExerciseStatisticsViewModel : ViewModel
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        private int _sets;

        public int Sets
        {
            get => _sets;
            set => SetField(ref _sets, value);
        }

        private int _repetitions;

        public int Repetitions
        {
            get => _repetitions;
            set => SetField(ref _repetitions, value);
        }

        public ExerciseStatisticsViewModel(ExerciseStatistics exerciseStatistics)
        {
            Name = exerciseStatistics.Exercise.Name;
            Sets = exerciseStatistics.Sets;
            Repetitions = exerciseStatistics.Repetitions;
        }
    }
}
