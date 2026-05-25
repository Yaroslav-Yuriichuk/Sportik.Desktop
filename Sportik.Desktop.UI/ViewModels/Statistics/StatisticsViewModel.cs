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

        public ImportViewModel Import { get; } = new ImportViewModel();

        public ICommand ShowImportCommand { get; }

        private IExerciseStatisticsService ExerciseStatisticsService => App.ServiceProvider.GetRequiredService<IExerciseStatisticsService>();
        private IEventsService EventsService => App.ServiceProvider.GetRequiredService<IEventsService>();

        private readonly CancellationTokenSource _loadCts = new CancellationTokenSource();

        public StatisticsViewModel()
        {
            ShowImportCommand = new ReactiveRelayCommand(OpenImport);

            _ = LoadDayStatisticsAsync(_loadCts.Token);

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

            Import.Dispose();
        }

        private async Task LoadDayStatisticsAsync(CancellationToken cancellationToken)
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

        private void OpenImport()
        {
            Import.Open();
        }

        private void EventsService_Event(ExerciseSetAddedEventArgs args)
        {
            DateTime date = args.Set.LoggedAt.Date;

            WeekStatisticsViewModel weekStatisticsViewModel =
                WeekStatistics.FirstOrDefault(vm => CalendarHelper.IsBeetween(date, vm.FirstWeekDayDate, vm.LastWeekDayDate));

            if (weekStatisticsViewModel == null)
            {
                weekStatisticsViewModel = new WeekStatisticsViewModel(new WeekStatistics(new List<DayStatistics>()));

                WeekStatistics.Add(weekStatisticsViewModel);

                WeekStatistics = new ObservableCollection<WeekStatisticsViewModel>(
                    WeekStatistics.OrderByDescending(vm => vm.FirstWeekDayDate));
            }

            weekStatisticsViewModel.AddSet(args.Set);
        }
    }
}
