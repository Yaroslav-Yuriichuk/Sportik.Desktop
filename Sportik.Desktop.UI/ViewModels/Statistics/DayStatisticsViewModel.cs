using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Sportik.Desktop.Core.Models.Statistics;

namespace Sportik.Desktop.UI.ViewModels.Statistics
{
    internal sealed class DayStatisticsViewModel : ViewModel, IDisposable
    {
        private DateTime _date;

        public DateTime Date
        {
            get => _date;
            set => SetField(ref _date, value);
        }

        private ObservableCollection<ExerciseStatisticsViewModel> _exerciseStatistics;

        public ObservableCollection<ExerciseStatisticsViewModel> ExerciseStatistics
        {
            get => _exerciseStatistics;
            set => SetField(ref _exerciseStatistics, value);
        }

        private bool _isCollapsed;

        public bool IsCollapsed
        {
            get => _isCollapsed;
            set => SetField(ref _isCollapsed, value);
        }

        public ICommand ToggleCollapsedCommand { get; private set; }

        public DayStatisticsViewModel(DayStatistics dayStatistics)
        {
            Date = dayStatistics.Date;

            ExerciseStatistics = new ObservableCollection<ExerciseStatisticsViewModel>(
                               dayStatistics.ExerciseStatistics.Select(statistics => new ExerciseStatisticsViewModel(statistics, dayStatistics.Date)));

            IsCollapsed = true;
            ToggleCollapsedCommand = new ReactiveRelayCommand(ToggleCollapsed);
        }

        public void Dispose()
        {
            foreach (ExerciseStatisticsViewModel exerciseStatisticsViewModel in ExerciseStatistics)
            {
                exerciseStatisticsViewModel.Dispose();
            }
        }

        private void ToggleCollapsed()
        {
            IsCollapsed = !IsCollapsed;
        }
    }
}
