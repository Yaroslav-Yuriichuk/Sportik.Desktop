using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.UI.ViewModels.Settings
{
    internal sealed class SettingsViewModel : ViewModel, IDisposable
    {
        private ObservableCollection<ExerciseSettingsViewModel> _exerciseSettings = new ObservableCollection<ExerciseSettingsViewModel>();

        public ObservableCollection<ExerciseSettingsViewModel> ExerciseSettings
        {
            get => _exerciseSettings;
            private set => SetField(ref _exerciseSettings, value);
        }

        private IExercisesService ExercisesService => App.ServiceProvider.GetRequiredService<IExercisesService>();

        private readonly CancellationTokenSource _loadCts = new CancellationTokenSource();

        public SettingsViewModel()
        {
            _ = LoadExerciseSettingsAsync(_loadCts.Token);
        }

        public void Dispose()
        {
            _loadCts.Cancel();

            foreach (ExerciseSettingsViewModel exerciseSettingsViewModel in ExerciseSettings)
            {
                exerciseSettingsViewModel.Dispose();
            }
        }

        private async Task LoadExerciseSettingsAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Exercise> exercises = await ExercisesService.GetAllAsync(cancellationToken);

            ExerciseSettings = new ObservableCollection<ExerciseSettingsViewModel>(
                exercises.Select(exercise => new ExerciseSettingsViewModel(exercise)));
        }
    }
}
