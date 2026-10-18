using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Statistics;

namespace Sportik.Desktop.UI.ViewModels.Statistics
{
    internal sealed class YearStatisticsViewModel : ViewModel, IDisposable
    {
        private DateTime _firstYearDate;

        public DateTime FirstYearDate
        {
            get => _firstYearDate;
            private set => SetField(ref _firstYearDate, value);
        }

        private DateTime _lastYearDate;

        public DateTime LastYearDate
        {
            get => _lastYearDate;
            private set => SetField(ref _lastYearDate, value);
        }

        private ObservableCollection<MonthStatisticsViewModel> _monthStatistics;

        public ObservableCollection<MonthStatisticsViewModel> MonthStatistics
        {
            get => _monthStatistics;
            private set => SetField(ref _monthStatistics, value);
        }

        public YearStatisticsViewModel(YearStatistics yearStatistics)
        {
            MonthStatistics = new ObservableCollection<MonthStatisticsViewModel>(
                                  yearStatistics.MonthStatistics.Select(statistics => new MonthStatisticsViewModel(statistics)));

            FirstYearDate = yearStatistics.FirstYearDate;
            LastYearDate = yearStatistics.LastYearDate;
        }

        public void Dispose()
        {
            foreach (MonthStatisticsViewModel monthStatisticsViewModel in MonthStatistics)
            {
                monthStatisticsViewModel.Dispose();
            }
        }

        public void AddSet(ExerciseSet set)
        {
            DateTime date = set.LoggedAt.Date;

            if (!CalendarHelper.IsBetween(date, FirstYearDate, LastYearDate))
            {
                return;
            }

            MonthStatisticsViewModel monthStatisticsViewModel = MonthStatistics
                .FirstOrDefault(statistics => CalendarHelper.IsBetween(date, statistics.FirstMonthDayDate, statistics.LastMonthDayDate));

            if (monthStatisticsViewModel == null)
            {
                DateTime firstMonthDayDate = CalendarHelper.GetFirstDayOfMonth(date);
                DateTime lastMonthDayDate = CalendarHelper.GetLastDayOfMonth(firstMonthDayDate);

                monthStatisticsViewModel = new MonthStatisticsViewModel(new MonthStatistics(firstMonthDayDate, lastMonthDayDate, new List<WeekStatistics>()));

                MonthStatistics.Add(monthStatisticsViewModel);

                MonthStatistics = new ObservableCollection<MonthStatisticsViewModel>(
                    MonthStatistics.OrderByDescending(statistics => statistics.FirstMonthDayDate));
            }

            monthStatisticsViewModel.AddSet(set);
        }
    }
}