using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Statistics;

namespace Sportik.Desktop.UI.ViewModels.Statistics
{
    internal sealed class MonthStatisticsViewModel : ViewModel, IDisposable
    {
        private DateTime _firstMonthDayDate;

        public DateTime FirstMonthDayDate
        {
            get => _firstMonthDayDate;
            private set => SetField(ref _firstMonthDayDate, value);
        }

        private DateTime _lastMonthDayDate;

        public DateTime LastMonthDayDate
        {
            get => _lastMonthDayDate;
            private set => SetField(ref _lastMonthDayDate, value);
        }

        private ObservableCollection<WeekStatisticsViewModel> _weekStatistics;

        public ObservableCollection<WeekStatisticsViewModel> WeekStatistics
        {
            get => _weekStatistics;
            private set => SetField(ref _weekStatistics, value);
        }

        public MonthStatisticsViewModel(MonthStatistics monthStatistics)
        {
            WeekStatistics = new ObservableCollection<WeekStatisticsViewModel>(
                               monthStatistics.WeekStatistics.Select(statistics => new WeekStatisticsViewModel(statistics)));

            FirstMonthDayDate = monthStatistics.FirstMonthDayDate;
            LastMonthDayDate = monthStatistics.LastMonthDayDate;
        }

        public void Dispose()
        {
            foreach (WeekStatisticsViewModel weekStatisticsViewModel in WeekStatistics)
            {
                weekStatisticsViewModel.Dispose();
            }
        }

        public void AddSet(ExerciseSet set)
        {
            DateTime date = set.LoggedAt.Date;

            if (!CalendarHelper.IsBetween(date, FirstMonthDayDate, LastMonthDayDate))
            {
                return;
            }

            WeekStatisticsViewModel weekStatisticsViewModel = WeekStatistics
                .FirstOrDefault(statistics => CalendarHelper.IsBetween(date, statistics.FirstWeekDayDate, statistics.LastWeekDayDate));

            if (weekStatisticsViewModel == null)
            {
                DateTime firstWeekDayDate = CalendarHelper.GetFirstDayOfWeek(date);
                DateTime lastWeekDayDate = CalendarHelper.GetLastDayOfWeek(firstWeekDayDate);

                weekStatisticsViewModel = new WeekStatisticsViewModel(new WeekStatistics(firstWeekDayDate, lastWeekDayDate, new List<DayStatistics>()));

                WeekStatistics.Add(weekStatisticsViewModel);

                WeekStatistics = new ObservableCollection<WeekStatisticsViewModel>(
                    WeekStatistics.OrderByDescending(statistics => statistics.FirstWeekDayDate));
            }

            weekStatisticsViewModel.AddSet(set);
        }
    }
}
