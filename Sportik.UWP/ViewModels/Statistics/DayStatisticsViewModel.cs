using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Sportik.Core.Models.Statistics;

namespace Sportik.UWP.ViewModels.Statistics
{
    internal sealed class DayStatisticsViewModel : ViewModel
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
                               dayStatistics.ExerciseStatistics.Select(statistics => new ExerciseStatisticsViewModel(statistics)));

            IsCollapsed = true;
            ToggleCollapsedCommand = new RelayCommand<object>(ToggleCollapsed);
        }

        private void ToggleCollapsed(object _)
        {
            IsCollapsed = !IsCollapsed;
        }
    }
}
