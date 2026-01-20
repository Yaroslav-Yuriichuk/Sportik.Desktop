using System;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.UI.ViewModels.Training
{
    internal sealed class TrainingViewModel : ViewModel, IDisposable
    {
        private TrainingSetupViewModel _trainingSetupViewModel;

        public TrainingSetupViewModel TrainingSetupViewModel
        {
            get => _trainingSetupViewModel;
            private set => SetField(ref _trainingSetupViewModel, value);
        }

        private TrainingExecutionViewModel _trainingExecutionViewModel;

        public TrainingExecutionViewModel TrainingExecutionViewModel
        {
            get => _trainingExecutionViewModel;
            private set => SetField(ref _trainingExecutionViewModel, value);
        }

        private bool _isTrainingRunning;

        public bool IsTrainingRunning
        {
            get => _isTrainingRunning;
            private set => SetField(ref _isTrainingRunning, value);
        }

        private ITrainingService TrainingService => App.ServiceProvider.GetRequiredService<ITrainingService>();

        public TrainingViewModel()
        {
            TrainingSetupViewModel = new TrainingSetupViewModel();
            TrainingExecutionViewModel = new TrainingExecutionViewModel();

            IsTrainingRunning = TrainingService.IsRunning;

            TrainingService.RunningStateChanged += SwitchMode;
        }

        public void Dispose()
        {
            TrainingService.RunningStateChanged -= SwitchMode;

            TrainingSetupViewModel.Dispose();
            TrainingExecutionViewModel.Dispose();
        }

        private void SwitchMode(TrainingRunningStateChangedEventArgs args)
        {
            IsTrainingRunning = args.IsRunning;
        }
    }
}