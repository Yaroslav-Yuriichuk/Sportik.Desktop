using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.UI.ViewModels.Settings
{
    internal sealed class SettingsViewModel : ViewModel, IDisposable
    {
        private ObservableCollection<ExerciseSettingsViewModel> _exerciseSettings;

        public ObservableCollection<ExerciseSettingsViewModel> ExerciseSettings
        {
            get => _exerciseSettings;
            set => SetField(ref _exerciseSettings, value);
        }

        private IExerciseSettingsService ExerciseSettingsService => App.ServiceProvider.GetService<IExerciseSettingsService>();

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
            IEnumerable<ExerciseSettings> exerciseSettings = await ExerciseSettingsService.GetAllExerciseSettingsAsync(cancellationToken);

            ExerciseSettings = new ObservableCollection<ExerciseSettingsViewModel>(
                exerciseSettings.Select(settings => new ExerciseSettingsViewModel(settings)));
        }
    }
}
