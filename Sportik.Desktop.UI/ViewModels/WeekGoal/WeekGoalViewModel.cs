using System;
using System.Collections.ObjectModel;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;

namespace Sportik.Desktop.UI.ViewModels.WeekGoal
{
    internal sealed class WeekGoalViewModel : ViewModel, IDisposable
    {
        public ObservableCollection<ExerciseGoalViewModel> ExerciseGoals
        {
            get => _exerciseGoals;
            private set => SetField(ref _exerciseGoals, value);
        }

        public AddExerciseGoalViewModel AddExerciseGoal { get; } = new AddExerciseGoalViewModel();

        public ReactiveRelayCommand OpenAddExerciseGoalCommand { get; }

        private ObservableCollection<ExerciseGoalViewModel> _exerciseGoals = new ObservableCollection<ExerciseGoalViewModel>();

        public WeekGoalViewModel()
        {
            ExerciseGoals = new ObservableCollection<ExerciseGoalViewModel>
            {
                new ExerciseGoalViewModel(new Exercise(Guid.NewGuid(), "Push-ups", new ExerciseSettings(false, 20, TimeSpan.Zero, TimeSpan.Zero))),
                new ExerciseGoalViewModel(new Exercise(Guid.NewGuid(), "Something", new ExerciseSettings(false, 20, TimeSpan.Zero, TimeSpan.Zero))),
            };

            OpenAddExerciseGoalCommand = new ReactiveRelayCommand(OpenAddExerciseGoal);
        }

        public void Dispose()
        {

        }

        private void OpenAddExerciseGoal()
        {
            AddExerciseGoal.Open();
        }
    }
}