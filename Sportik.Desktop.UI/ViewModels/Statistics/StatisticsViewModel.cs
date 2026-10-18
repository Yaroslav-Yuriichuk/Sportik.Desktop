using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.UI.Models.Statistics;

namespace Sportik.Desktop.UI.ViewModels.Statistics
{
    internal sealed class StatisticsViewModel : ViewModel, IDisposable
    {
        private ObservableCollection<WeekStatisticsViewModel> _weekStatistics = new ObservableCollection<WeekStatisticsViewModel>();

        public ObservableCollection<WeekStatisticsViewModel> WeekStatistics
        {
            get => _weekStatistics;
            private set => SetField(ref _weekStatistics, value);
        }

        private ObservableCollection<MonthStatisticsViewModel> _monthStatistics = new ObservableCollection<MonthStatisticsViewModel>();

        public ObservableCollection<MonthStatisticsViewModel> MonthStatistics
        {
            get => _monthStatistics;
            private set => SetField(ref _monthStatistics, value);
        }

        private ObservableCollection<StatisticsGroupingOption> _groupingOptions;

        public ObservableCollection<StatisticsGroupingOption> GroupingOptions
        {
            get => _groupingOptions;
            private set
            {
                if (SetField(ref _groupingOptions, value))
                {
                    SetField(ref _selectedGroupingOption, value[0], nameof(SelectedGroupingOption));
                }
            }
        }

        private StatisticsGroupingOption _selectedGroupingOption;

        public StatisticsGroupingOption SelectedGroupingOption
        {
            get => _selectedGroupingOption;
            set
            {
                if (SetField(ref _selectedGroupingOption, value))
                {
                    _ = LoadStatisticsAsync(SelectedGroupingOption.Grouping, _loadCts.Token);
                }
            }
        }

        public ImportViewModel Import { get; } = new ImportViewModel();

        public ExportViewModel Export { get; } = new ExportViewModel();

        public ICommand ShowImportCommand { get; }

        public ICommand ShowExportCommand { get; }

        private IExerciseStatisticsService ExerciseStatisticsService => App.ServiceProvider.GetRequiredService<IExerciseStatisticsService>();
        private IEventsService EventsService => App.ServiceProvider.GetRequiredService<IEventsService>();

        private readonly CancellationTokenSource _loadCts = new CancellationTokenSource();

        public StatisticsViewModel()
        {
            ShowImportCommand = new ReactiveRelayCommand(OpenImport);
            ShowExportCommand = new ReactiveRelayCommand(OpenExport);

            GroupingOptions = new ObservableCollection<StatisticsGroupingOption>
            {
                new StatisticsGroupingOption("Week", StatisticsGrouping.Week),
                new StatisticsGroupingOption("Month", StatisticsGrouping.Month),
            };

            _ = LoadStatisticsAsync(SelectedGroupingOption.Grouping, _loadCts.Token);

            EventsService.AddListener<ExerciseSetAddedEventArgs>(EventsService_Event);
        }

        public void Dispose()
        {
            _loadCts.Cancel();

            EventsService.RemoveListener<ExerciseSetAddedEventArgs>(EventsService_Event);

            foreach (WeekStatisticsViewModel weekStatisticsViewModel in WeekStatistics)
            {
                weekStatisticsViewModel.Dispose();
            }

            foreach (MonthStatisticsViewModel monthStatisticsViewModel in MonthStatistics)
            {
                monthStatisticsViewModel.Dispose();
            }

            Import.Dispose();
            Export.Dispose();
        }

        private async Task LoadStatisticsAsync(StatisticsGrouping grouping, CancellationToken cancellationToken)
        {
            if (grouping == StatisticsGrouping.Month)
            {
                await LoadMonthStatisticsAsync(cancellationToken);
            }
            else
            {
                await LoadWeekStatisticsAsync(cancellationToken);
            }
        }

        private async Task LoadWeekStatisticsAsync(CancellationToken cancellationToken)
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

        private async Task LoadMonthStatisticsAsync(CancellationToken cancellationToken)
        {
            OperationResult<IEnumerable<MonthStatistics>> result = await ExerciseStatisticsService.GetMonthlyAsync(cancellationToken);

            if (!result.Succeeded)
            {
                // TODO: Handle error.
                return;
            }

            IEnumerable<MonthStatistics> monthStatistics = result.Value;

            MonthStatistics = new ObservableCollection<MonthStatisticsViewModel>(
                monthStatistics.Select(statistics => new MonthStatisticsViewModel(statistics)));
        }

        private void OpenImport()
        {
            Import.Open();
        }

        private void OpenExport()
        {
            Export.Open();
        }

        private void EventsService_Event(ExerciseSetAddedEventArgs args)
        {
            DateTime date = args.Set.LoggedAt.Date;

            if (SelectedGroupingOption.Grouping == StatisticsGrouping.Month)
            {
                MonthStatisticsViewModel monthStatisticsViewModel =
                    MonthStatistics.FirstOrDefault(vm => CalendarHelper.IsBetween(date, vm.FirstMonthDayDate, vm.LastMonthDayDate));

                if (monthStatisticsViewModel == null)
                {
                    DateTime firstMonthDayDate = CalendarHelper.GetFirstDayOfMonth(date);
                    DateTime lastMonthDayDate = CalendarHelper.GetLastDayOfMonth(firstMonthDayDate);

                    monthStatisticsViewModel = new MonthStatisticsViewModel(new MonthStatistics(firstMonthDayDate, lastMonthDayDate, new List<WeekStatistics>()));

                    MonthStatistics.Add(monthStatisticsViewModel);

                    MonthStatistics = new ObservableCollection<MonthStatisticsViewModel>(
                        MonthStatistics.OrderByDescending(vm => vm.FirstMonthDayDate));
                }

                monthStatisticsViewModel.AddSet(args.Set);
            }
            else
            {
                WeekStatisticsViewModel weekStatisticsViewModel =
                    WeekStatistics.FirstOrDefault(vm => CalendarHelper.IsBetween(date, vm.FirstWeekDayDate, vm.LastWeekDayDate));

                if (weekStatisticsViewModel == null)
                {
                    DateTime firstWeekDayDate = CalendarHelper.GetFirstDayOfWeek(date);
                    DateTime lastWeekDayDate = CalendarHelper.GetLastDayOfWeek(firstWeekDayDate);

                    weekStatisticsViewModel = new WeekStatisticsViewModel(new WeekStatistics(firstWeekDayDate, lastWeekDayDate, new List<DayStatistics>()));

                    WeekStatistics.Add(weekStatisticsViewModel);

                    WeekStatistics = new ObservableCollection<WeekStatisticsViewModel>(
                        WeekStatistics.OrderByDescending(vm => vm.FirstWeekDayDate));
                }

                weekStatisticsViewModel.AddSet(args.Set);
            }
        }
    }
}
