using System;
using System.Collections.ObjectModel;
using System.Linq;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models.Statistics;

namespace Sportik.Desktop.UI.ViewModels.Statistics
{
    internal sealed class WeekStatisticsViewModel : ViewModel, IDisposable
    {
        private DateTime _firstWeekDayDate;

        public DateTime FirstWeekDayDate
        {
            get => _firstWeekDayDate;
            set => SetField(ref _firstWeekDayDate, value);
        }

        private DateTime _lastWeekDayDate;

        public DateTime LastWeekDayDate
        {
            get => _lastWeekDayDate;
            set => SetField(ref _lastWeekDayDate, value);
        }

        private ObservableCollection<DayStatisticsViewModel> _dayStatistics;

        public ObservableCollection<DayStatisticsViewModel> DayStatistics
        {
            get => _dayStatistics;
            set => SetField(ref _dayStatistics, value);
        }

        public WeekStatisticsViewModel(WeekStatistics weekStatistics)
        {
            DayStatistics = new ObservableCollection<DayStatisticsViewModel>(
                               weekStatistics.DayStatistics.Select(statistics => new DayStatisticsViewModel(statistics)));

            FirstWeekDayDate = StatisticsHelper.GetWeekFirstDayDate(weekStatistics);
            LastWeekDayDate = StatisticsHelper.GetWeekLastDayDate(weekStatistics);
        }

        public void Dispose()
        {
            foreach (DayStatisticsViewModel dayStatisticsViewModel in DayStatistics)
            {
                dayStatisticsViewModel.Dispose();
            }
        }
    }
}
