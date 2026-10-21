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
    internal sealed class MonthSetsBarChartLoader : IDashboardBarChartLoader
    {
        private readonly IExerciseStatisticsService _exerciseStatisticsService;
        private readonly int _year;

        public MonthSetsBarChartLoader(IExerciseStatisticsService exerciseStatisticsService, int year)
        {
            _exerciseStatisticsService = exerciseStatisticsService;
            _year = year;
        }

        public async Task<OperationResult<IEnumerable<BarChartColumn>>> LoadBarChartAsync(CancellationToken cancellationToken)
        {
            OperationResult<IEnumerable<AggregatedSetsMonthStatistics>> result =
                await _exerciseStatisticsService.GetAggregatedMonthSetsAsync(_year, cancellationToken);

            if (!result.Succeeded)
            {
                return OperationResult<IEnumerable<BarChartColumn>>.Failure(result.Errors);
            }

            return OperationResult<IEnumerable<BarChartColumn>>.Success(
                result.Value.Select(s => new BarChartColumn(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(s.Month), s.TotalSets)));
        }
    }
}