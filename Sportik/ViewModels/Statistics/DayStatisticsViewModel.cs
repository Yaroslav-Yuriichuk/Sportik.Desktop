using System;
using System.Collections.ObjectModel;
using System.Linq;
using Sportik.Models.Statistics;

namespace Sportik.ViewModels.Statistics
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

        public DayStatisticsViewModel(DayStatistics dayStatistics)
        {
            Date = dayStatistics.Date;

            ExerciseStatistics = new ObservableCollection<ExerciseStatisticsViewModel>(
                               dayStatistics.ExerciseStatistics.Select(statistics => new ExerciseStatisticsViewModel(statistics)));
        }
    }
}
