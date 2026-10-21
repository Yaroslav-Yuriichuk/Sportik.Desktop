using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Extensions;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.UI.Constants;
using Sportik.Desktop.UI.Models.Dashboard;
using Sportik.Desktop.UI.ViewModels.Dashboard.BarChartLoaders;

namespace Sportik.Desktop.UI.ViewModels.Dashboard
{
    internal sealed class DashboardViewModel : ViewModel, IDisposable
    {
        private ObservableCollection<BarChartColumn> _barChartData = new ObservableCollection<BarChartColumn>();

        public ObservableCollection<BarChartColumn> BarChartData
        {
            get => _barChartData;
            private set => SetField(ref _barChartData, value);
        }

        private ObservableCollection<DashboardGroupingOption> _groupingOptions;

        public ObservableCollection<DashboardGroupingOption> GroupingOptions
        {
            get => _groupingOptions;
            private set
            {
                if (SetField(ref _groupingOptions, value))
                {
                    SetField(ref _selectedGroupingOption, value[0], nameof(SelectedGroupingOption));
                    TargetOptions = new ObservableCollection<DashboardTargetOption>(SelectedGroupingOption.TargetOptions);
                }
            }
        }

        private DashboardGroupingOption _selectedGroupingOption;

        public DashboardGroupingOption SelectedGroupingOption
        {
            get => _selectedGroupingOption;
            set
            {
                if (SetField(ref _selectedGroupingOption, value))
                {
                    TargetOptions = new ObservableCollection<DashboardTargetOption>(SelectedGroupingOption.TargetOptions);

                    DashboardCache dashboardCache = PersistentCacheService.GetOrNew<DashboardCache>();

                    dashboardCache.LastGrouping = SelectedGroupingOption.Grouping;
                    dashboardCache.LastTarget = SelectedTargetOption.Target;

                    PersistentCacheService.Set(dashboardCache);

                    _loadCts.Cancel();
                    _loadCts = new CancellationTokenSource();

                    _ = LoadDashboardAsync(SelectedGroupingOption.Grouping, SelectedTargetOption.Target, _loadCts.Token);
                }
            }
        }

        private ObservableCollection<DashboardTargetOption> _targetOptions;

        public ObservableCollection<DashboardTargetOption> TargetOptions
        {
            get => _targetOptions;
            private set
            {
                if (SetField(ref _targetOptions, value))
                {
                    SetField(ref _selectedTargetOption, value[0], nameof(SelectedTargetOption));
                }
            }
        }

        private DashboardTargetOption _selectedTargetOption;

        public DashboardTargetOption SelectedTargetOption
        {
            get => _selectedTargetOption;
            set
            {
                if (SetField(ref _selectedTargetOption, value))
                {
                    if (value == null)
                    {
                        return;
                    }

                    DashboardCache dashboardCache = PersistentCacheService.GetOrNew<DashboardCache>();

                    dashboardCache.LastGrouping = SelectedGroupingOption.Grouping;
                    dashboardCache.LastTarget = SelectedTargetOption.Target;

                    PersistentCacheService.Set(dashboardCache);

                    _loadCts.Cancel();
                    _loadCts = new CancellationTokenSource();

                    _ = LoadDashboardAsync(SelectedGroupingOption.Grouping, SelectedTargetOption.Target, _loadCts.Token);
                }
            }
        }

        private ObservableCollection<DashboardYearOption> _yearOptions;

        public ObservableCollection<DashboardYearOption> YearOptions
        {
            get => _yearOptions;
            private set
            {
                if (SetField(ref _yearOptions, value))
                {
                    SetField(ref _selectedYearOption, value[0], nameof(SelectedYearOption));
                }
            }
        }

        private DashboardYearOption _selectedYearOption;

        public DashboardYearOption SelectedYearOption
        {
            get => _selectedYearOption;
            set
            {
                if (SetField(ref _selectedYearOption, value))
                {
                    if (value == null || SelectedGroupingOption?.Grouping != DashboardGrouping.Month)
                    {
                        return;
                    }

                    DashboardCache dashboardCache = new DashboardCache
                    {
                        LastGrouping = SelectedGroupingOption.Grouping,
                        LastTarget = SelectedTargetOption.Target,
                        LastYear = SelectedYearOption.Year
                    };

                    PersistentCacheService.Set(dashboardCache);

                    _loadCts.Cancel();
                    _loadCts = new CancellationTokenSource();

                    _ = LoadDashboardAsync(SelectedGroupingOption.Grouping, SelectedTargetOption.Target, _loadCts.Token);
                }
            }
        }

        private IExerciseStatisticsService ExerciseStatisticsService => App.ServiceProvider.GetRequiredService<IExerciseStatisticsService>();
        private IPersistentCacheService PersistentCacheService => App.ServiceProvider.GetRequiredService<IPersistentCacheService>();

        private CancellationTokenSource _loadCts = new CancellationTokenSource();

        public DashboardViewModel()
        {
            List<DashboardTargetOption> exerciseTargetOptions = new List<DashboardTargetOption>
            {
                new DashboardTargetOption("Repetitions", DashboardTarget.Repetitions),
                new DashboardTargetOption("Sets", DashboardTarget.Sets),
            };

            List<DashboardTargetOption> dayTargetOptions = new List<DashboardTargetOption>
            {
                new DashboardTargetOption("Sets", DashboardTarget.Sets),
            };

            List<DashboardTargetOption> monthTargetOptions = new List<DashboardTargetOption>
            {
                new DashboardTargetOption("Sets", DashboardTarget.Sets),
            };

            List<DashboardGroupingOption> groupingOptions = new List<DashboardGroupingOption>
            {
                new DashboardGroupingOption("Exercise", DashboardGrouping.Exercise, exerciseTargetOptions),
                new DashboardGroupingOption("Day of Week", DashboardGrouping.Day, dayTargetOptions),
                new DashboardGroupingOption("Month", DashboardGrouping.Month, monthTargetOptions),
            };

            List<DashboardYearOption> yearOptions = DashboardConstants.Years
                .Select(year => new DashboardYearOption(year.ToString(), year))
                .ToList();

            DashboardGroupingOption selectedGroupingOption = groupingOptions[0];
            DashboardYearOption selectedYearOption = yearOptions.First();

            List<DashboardTargetOption> targetOptions = selectedGroupingOption.TargetOptions.ToList();
            DashboardTargetOption selectedTargetOption = targetOptions.First();

            if (PersistentCacheService.TryGet(out DashboardCache dashboardCache))
            {
                DashboardGroupingOption lastSelectedGroupingOption = groupingOptions.FirstOrDefault(go => go.Grouping == dashboardCache.LastGrouping);
                selectedGroupingOption = lastSelectedGroupingOption ?? selectedGroupingOption;

                targetOptions = selectedGroupingOption.TargetOptions.ToList();

                DashboardTargetOption lastSelectedTargetOption = targetOptions.FirstOrDefault(to => to.Target == dashboardCache.LastTarget);
                selectedTargetOption = lastSelectedTargetOption ?? targetOptions.First();

                DashboardYearOption lastSelectedYearOption = yearOptions.FirstOrDefault(yo => yo.Year == dashboardCache.LastYear);
                selectedYearOption = lastSelectedYearOption ?? selectedYearOption;
            }

            SetField(ref _yearOptions, new ObservableCollection<DashboardYearOption>(yearOptions), nameof(YearOptions));
            SetField(ref _selectedYearOption, selectedYearOption, nameof(SelectedYearOption));

            SetField(ref _groupingOptions, new ObservableCollection<DashboardGroupingOption>(groupingOptions), nameof(GroupingOptions));
            SetField(ref _selectedGroupingOption, selectedGroupingOption, nameof(SelectedGroupingOption));

            SetField(ref _targetOptions, new ObservableCollection<DashboardTargetOption>(targetOptions), nameof(TargetOptions));
            SetField(ref _selectedTargetOption, selectedTargetOption, nameof(SelectedTargetOption));

            _ = LoadDashboardAsync(SelectedGroupingOption.Grouping, SelectedTargetOption.Target, _loadCts.Token);
        }

        public void Dispose()
        {
            _loadCts.Cancel();
            _loadCts.Dispose();
        }

        private async Task LoadDashboardAsync(DashboardGrouping grouping, DashboardTarget target, CancellationToken cancellationToken)
        {
            IDashboardBarChartLoader barChartLoader = grouping switch
            {
                DashboardGrouping.Exercise when target == DashboardTarget.Repetitions => new ExerciseRepetitionsBarChartLoader(ExerciseStatisticsService),
                DashboardGrouping.Exercise when target == DashboardTarget.Sets => new ExerciseSetsBarChartLoader(ExerciseStatisticsService),
                DashboardGrouping.Day when target == DashboardTarget.Sets => new DaySetsBarChartLoader(ExerciseStatisticsService),
                DashboardGrouping.Month when target == DashboardTarget.Sets => new MonthSetsBarChartLoader(ExerciseStatisticsService, SelectedYearOption.Year),
                _ => throw new ArgumentOutOfRangeException(nameof(grouping), grouping, null)
            };

            OperationResult<IEnumerable<BarChartColumn>> result = await barChartLoader.LoadBarChartAsync(cancellationToken);

            if (!result.Succeeded)
            {
                // TODO: Handle error.
                return;
            }

            BarChartData = new ObservableCollection<BarChartColumn>(result.Value);
        }
    }
}