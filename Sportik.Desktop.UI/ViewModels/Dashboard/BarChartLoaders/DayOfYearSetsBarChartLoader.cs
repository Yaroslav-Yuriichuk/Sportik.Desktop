using System.Collections.Generic;
using System.Globalization;
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
    internal sealed class DayOfYearSetsBarChartLoader : IDashboardBarChartLoader
    {
        private readonly IExerciseStatisticsService _exerciseStatisticsService;
        private readonly int _year;

        public DayOfYearSetsBarChartLoader(IExerciseStatisticsService exerciseStatisticsService, int year)
        {
            _exerciseStatisticsService = exerciseStatisticsService;
            _year = year;
        }

        public async Task<OperationResult<IEnumerable<BarChartColumn>>> LoadBarChartAsync(CancellationToken cancellationToken)
        {
            OperationResult<IEnumerable<AggregatedSetsDayOfYearStatistics>> result =
                await _exerciseStatisticsService.GetAggregatedDayOfYearSetsAsync(_year, cancellationToken);

            if (!result.Succeeded)
            {
                return OperationResult<IEnumerable<BarChartColumn>>.Failure(result.Errors);
            }

            return OperationResult<IEnumerable<BarChartColumn>>.Success(
                result.Value.Select(s => new BarChartColumn(
                    s.Date.Day == 1 ? s.Date.ToString("MMM d", CultureInfo.CurrentCulture) : string.Empty,
                    s.TotalSets)));
        }
    }
}
