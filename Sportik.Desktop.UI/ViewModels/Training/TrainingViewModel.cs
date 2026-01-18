using System;

namespace Sportik.Desktop.UI.ViewModels.Training
{
    internal sealed class TrainingViewModel : ViewModel, IDisposable
    {
        private TrainingSetupViewModel _trainingSetupViewModel;

        public TrainingSetupViewModel TrainingSetupViewModel
        {
            get => _trainingSetupViewModel;
            set => SetField(ref _trainingSetupViewModel, value);
        }

        private TrainingExecutionViewModel _trainingExecutionViewModel;

        public TrainingExecutionViewModel TrainingExecutionViewModel
        {
            get => _trainingExecutionViewModel;
            set => SetField(ref _trainingExecutionViewModel, value);
        }

        public TrainingViewModel()
        {
            TrainingSetupViewModel = new TrainingSetupViewModel();
            TrainingExecutionViewModel = new TrainingExecutionViewModel();
        }

        public void Dispose()
        {
            TrainingSetupViewModel.Dispose();
            TrainingExecutionViewModel.Dispose();
        }
    }
}