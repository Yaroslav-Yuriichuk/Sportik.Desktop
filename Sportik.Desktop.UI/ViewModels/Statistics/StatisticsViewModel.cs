using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.UI.ViewModels.Statistics
{
    internal sealed class StatisticsViewModel : ViewModel, IDisposable
    {
        private ObservableCollection<WeekStatisticsViewModel> _weekStatistics = new ObservableCollection<WeekStatisticsViewModel>();

        public ObservableCollection<WeekStatisticsViewModel> WeekStatistics
        {
            get => _weekStatistics;
            set => SetField(ref _weekStatistics, value);
        }

        private IExerciseStatisticsService ExerciseStatisticsService => App.ServiceProvider.GetRequiredService<IExerciseStatisticsService>();

        private readonly CancellationTokenSource _loadCts = new CancellationTokenSource();

        private bool _isImportOpen;

        public bool IsImportOpen
        {
            get => _isImportOpen;
            set => SetField(ref _isImportOpen, value);
        }

        public ICommand ShowImportCommand { get; }
        public ICommand CloseImportCommand { get; }

        public StatisticsViewModel()
        {
            ShowImportCommand = new ReactiveRelayCommand(ShowImport);
            CloseImportCommand = new ReactiveRelayCommand(CloseImport);

            _ = LoadDayStatisticsAsync(_loadCts.Token);
        }

        public void Dispose()
        {
            _loadCts.Cancel();

            foreach (WeekStatisticsViewModel weekStatisticsViewModel in WeekStatistics)
            {
                weekStatisticsViewModel.Dispose();
            }
        }

        private async Task LoadDayStatisticsAsync(CancellationToken cancellationToken)
        {
            OperationResult<IEnumerable<WeekStatistics>> result = await ExerciseStatisticsService.GetWeeklyAsync(cancellationToken);

            if (!result.Succeeded)
            {
                // TODO: Handle error.
                return;
            }

            IEnumerable<WeekStatistics> weekStatistics = result.Value;

            WeekStatistics = new ObservableCollection<WeekStatisticsViewModel>(
                weekStatistics.Select(statistics => new WeekStatisticsViewModel(statistics)));
        }

        private void ShowImport()
        {
            IsImportOpen = true;
        }

        private void CloseImport()
        {
            IsImportOpen = false;
        }
    }
}
