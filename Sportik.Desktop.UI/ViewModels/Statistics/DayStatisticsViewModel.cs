using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.UI.ViewModels.Statistics
{
    internal sealed class DayStatisticsViewModel : ViewModel, IDisposable
    {
        private DateTime _date;

        public DateTime Date
        {
            get => _date;
            private set => SetField(ref _date, value);
        }

        private ObservableCollection<ExerciseStatisticsViewModel> _exerciseStatistics;

        public ObservableCollection<ExerciseStatisticsViewModel> ExerciseStatistics
        {
            get => _exerciseStatistics;
            private set => SetField(ref _exerciseStatistics, value);
        }

        private bool _isCollapsed;

        public bool IsCollapsed
        {
            get => _isCollapsed;
            private set => SetField(ref _isCollapsed, value);
        }

        public ICommand ToggleCollapsedCommand { get; }

        private IExercisesService ExercisesService => App.ServiceProvider.GetRequiredService<IExercisesService>();

        private readonly CancellationTokenSource _loadCts = new CancellationTokenSource();

        public DayStatisticsViewModel(DayStatistics dayStatistics)
        {
            Date = dayStatistics.Date;

            ExerciseStatistics = new ObservableCollection<ExerciseStatisticsViewModel>(
                               dayStatistics.ExerciseStatistics.Select(statistics => new ExerciseStatisticsViewModel(statistics)));

            IsCollapsed = true;
            ToggleCollapsedCommand = new ReactiveRelayCommand(ToggleCollapsed);
        }

        public void Dispose()
        {
            _loadCts.Cancel();
            _loadCts.Dispose();

            foreach (ExerciseStatisticsViewModel exerciseStatisticsViewModel in ExerciseStatistics)
            {
                exerciseStatisticsViewModel.Dispose();
            }
        }

        public void AddSet(ExerciseSet set)
        {
            if (set.LoggedAt.Date != Date)
            {
                return;
            }

            ExerciseStatisticsViewModel exerciseStatisticsViewModel = ExerciseStatistics
                .FirstOrDefault(statistics => statistics.ExerciseId == set.ExerciseId);

            if (exerciseStatisticsViewModel != null)
            {
                exerciseStatisticsViewModel.AddSet(set);
                return;
            }

            _ = AddSetAsync(set, _loadCts.Token);
        }

        private async Task AddSetAsync(ExerciseSet set, CancellationToken cancellationToken)
        {
            OperationResult<Exercise> result = await ExercisesService.GetByIdAsync(set.ExerciseId, cancellationToken);

            if (!result.Succeeded)
            {
                // TODO: Handle error.
                return;
            }

            ExerciseStatisticsViewModel exerciseStatisticsViewModel =
                new ExerciseStatisticsViewModel(new ExerciseStatistics(result.Value, new List<ExerciseSet> { set }));

            ExerciseStatistics.Add(exerciseStatisticsViewModel);
        }

        private void ToggleCollapsed()
        {
            IsCollapsed = !IsCollapsed;
        }
    }
}
