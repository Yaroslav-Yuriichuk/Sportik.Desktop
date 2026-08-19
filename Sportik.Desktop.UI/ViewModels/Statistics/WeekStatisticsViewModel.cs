using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Statistics;

namespace Sportik.Desktop.UI.ViewModels.Statistics
{
    internal sealed class WeekStatisticsViewModel : ViewModel, IDisposable
    {
        private DateTime _firstWeekDayDate;

        public DateTime FirstWeekDayDate
        {
            get => _firstWeekDayDate;
            private set => SetField(ref _firstWeekDayDate, value);
        }

        private DateTime _lastWeekDayDate;

        public DateTime LastWeekDayDate
        {
            get => _lastWeekDayDate;
            private set => SetField(ref _lastWeekDayDate, value);
        }

        private ObservableCollection<DayStatisticsViewModel> _dayStatistics;

        public ObservableCollection<DayStatisticsViewModel> DayStatistics
        {
            get => _dayStatistics;
            private set => SetField(ref _dayStatistics, value);
        }

        public WeekStatisticsViewModel(WeekStatistics weekStatistics)
        {
            DayStatistics = new ObservableCollection<DayStatisticsViewModel>(
                               weekStatistics.DayStatistics.Select(statistics => new DayStatisticsViewModel(statistics)));

            FirstWeekDayDate = weekStatistics.FirstWeekDayDate;
            LastWeekDayDate = weekStatistics.LastWeekDayDate;
        }

        public void Dispose()
        {
            foreach (DayStatisticsViewModel dayStatisticsViewModel in DayStatistics)
            {
                dayStatisticsViewModel.Dispose();
            }
        }

        public void AddSet(ExerciseSet set)
        {
            DateTime date = set.LoggedAt.Date;

            if (!CalendarHelper.IsBetween(date, FirstWeekDayDate, LastWeekDayDate))
            {
                return;
            }

            DayStatisticsViewModel dayStatisticsViewModel = DayStatistics
                .FirstOrDefault(statistics => statistics.Date == date);

            if (dayStatisticsViewModel == null)
            {
                dayStatisticsViewModel = new DayStatisticsViewModel(new DayStatistics(date, new List<ExerciseStatistics>()));

                DayStatistics.Add(dayStatisticsViewModel);

                DayStatistics = new ObservableCollection<DayStatisticsViewModel>(
                    DayStatistics.OrderByDescending(statistics => statistics.Date));
            }

            dayStatisticsViewModel.AddSet(set);
        }
    }
}
