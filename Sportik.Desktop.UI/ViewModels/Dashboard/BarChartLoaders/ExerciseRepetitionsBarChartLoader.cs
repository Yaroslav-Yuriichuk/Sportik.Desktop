using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.UI.Models;

namespace Sportik.Desktop.UI.ViewModels.Dashboard.BarChartLoaders
{
    internal sealed class ExerciseRepetitionsBarChartLoader : IDashboardBarChartLoader
    {
        private readonly IExerciseStatisticsService _exerciseStatisticsService;

        public ExerciseRepetitionsBarChartLoader(IExerciseStatisticsService exerciseStatisticsService)
        {
            _exerciseStatisticsService = exerciseStatisticsService;
        }

        public async Task<OperationResult<IEnumerable<BarChartColumn>>> LoadBarChartAsync(CancellationToken cancellationToken)
        {
            OperationResult<IEnumerable<AggregatedRepetitionsExerciseStatistics>> result =
                await _exerciseStatisticsService.GetAggregatedExerciseRepetitionsAsync(cancellationToken);

            if (!result.Succeeded)
            {
                return OperationResult<IEnumerable<BarChartColumn>>.Failure(result.Errors);
            }

            return OperationResult<IEnumerable<BarChartColumn>>.Success(
                result.Value.Select(s => new BarChartColumn(s.Exercise.Name, s.TotalRepetitions)));
        }
    }
}