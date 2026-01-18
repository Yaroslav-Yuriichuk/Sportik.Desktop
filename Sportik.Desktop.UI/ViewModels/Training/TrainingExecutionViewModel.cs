using System;
using System.Collections.ObjectModel;

namespace Sportik.Desktop.UI.ViewModels.Training
{
    internal sealed class TrainingExecutionViewModel : ViewModel, IDisposable
    {
        private ObservableCollection<ExecutionSetViewModel> _sets;

        public ObservableCollection<ExecutionSetViewModel> Sets
        {
            get => _sets;
            set => SetField(ref _sets, value);
        }

        private ObservableCollection<TrainingExerciseViewModel> _exercises;

        public ObservableCollection<TrainingExerciseViewModel> Exercises
        {
            get => _exercises;
            set => SetField(ref _exercises, value);
        }

        public IReactiveCommand CancelTrainingCommand { get; }

        public TrainingExecutionViewModel()
        {
            CancelTrainingCommand = new ReactiveRelayCommand(CancelTraining);
        }

        public void Dispose() { }


        private void CancelTraining()
        {
        }
    }
}