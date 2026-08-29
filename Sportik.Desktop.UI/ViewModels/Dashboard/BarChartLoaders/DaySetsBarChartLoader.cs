using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.UI.Models;
using Sportik.Desktop.UI.Models.Dashboard;

namespace Sportik.Desktop.UI.ViewModels.Dashboard.BarChartLoaders
{
    internal sealed class DaySetsBarChartLoader : IDashboardBarChartLoader
    {
        private readonly IExerciseStatisticsService _exerciseStatisticsService;

        public DaySetsBarChartLoader(IExerciseStatisticsService exerciseStatisticsService)
        {
            _exerciseStatisticsService = exerciseStatisticsService;
        }

        public async Task<OperationResult<IEnumerable<BarChartColumn>>> LoadBarChartAsync(CancellationToken cancellationToken)
        {
            OperationResult<IEnumerable<AggregatedSetsDayStatistics>> result =
                await _exerciseStatisticsService.GetAggregatedDaySetsAsync(cancellationToken);

            if (!result.Succeeded)
            {
                return OperationResult<IEnumerable<BarChartColumn>>.Failure(result.Errors);
            }

            return OperationResult<IEnumerable<BarChartColumn>>.Success(
                result.Value.Select(s => new BarChartColumn(s.DayOfWeek, s.TotalSets)));
        }
    }
}