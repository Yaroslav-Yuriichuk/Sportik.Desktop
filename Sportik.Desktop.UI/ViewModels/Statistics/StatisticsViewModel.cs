using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sportik.Desktop.UI.ViewModels.Statistics
{
    internal sealed class StatisticsViewModel : ViewModel, IDisposable
    {
        private ObservableCollection<WeekStatisticsViewModel> _weekStatistics;

        public ObservableCollection<WeekStatisticsViewModel> WeekStatistics
        {
            get => _weekStatistics;
            set => SetField(ref _weekStatistics, value);
        }

        private IExerciseStatisticsService ExerciseStatisticsService => App.ServiceProvider.GetService<IExerciseStatisticsService>();

        private readonly CancellationTokenSource _loadCts = new CancellationTokenSource();

        public StatisticsViewModel()
        {
            _ = LoadDayStatisticsAsync(_loadCts.Token);
        }

        public void Dispose()
        {
            _loadCts.Cancel();
        }

        private async Task LoadDayStatisticsAsync(CancellationToken cancellationToken)
        {
            IEnumerable<WeekStatistics> weekStatistics = await ExerciseStatisticsService.GetWeekStatisticsAsync(WeekStatisticsOrder.Descending, cancellationToken);

            WeekStatistics = new ObservableCollection<WeekStatisticsViewModel>(
                               weekStatistics.Select(statistics => new WeekStatisticsViewModel(statistics)));
        }
    }
}
