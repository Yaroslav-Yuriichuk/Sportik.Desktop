using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.UI.Models;
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
                    _loadCts.Cancel();
                    _loadCts = new CancellationTokenSource();

                    _ = LoadDashboardAsync(SelectedGroupingOption.Grouping, SelectedTargetOption.Target, _loadCts.Token);
                }
            }
        }

        private IExerciseStatisticsService ExerciseStatisticsService => App.ServiceProvider.GetRequiredService<IExerciseStatisticsService>();

        private CancellationTokenSource _loadCts = new CancellationTokenSource();

        public DashboardViewModel()
        {
            GroupingOptions = new ObservableCollection<DashboardGroupingOption>
            {
                new DashboardGroupingOption("Exercise", DashboardGrouping.Exercise),
            };

            TargetOptions = new ObservableCollection<DashboardTargetOption>
            {
                new DashboardTargetOption("Repetitions", DashboardTarget.Repetitions),
                new DashboardTargetOption("Sets", DashboardTarget.Sets),
            };

            _ = LoadDashboardAsync(SelectedGroupingOption.Grouping, SelectedTargetOption.Target, _loadCts.Token);
        }

        public void Dispose()
        {
            _loadCts.Cancel();
            _loadCts.Dispose();
        }

        public async Task LoadDashboardAsync(DashboardGrouping grouping, DashboardTarget target, CancellationToken cancellationToken)
        {
            IDashboardBarChartLoader barChartLoader = grouping switch
            {
                DashboardGrouping.Exercise when target == DashboardTarget.Repetitions => new ExerciseRepetitionsBarChartLoader(ExerciseStatisticsService),
                DashboardGrouping.Exercise when target == DashboardTarget.Sets => new ExerciseSetsBarChartLoader(ExerciseStatisticsService),
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