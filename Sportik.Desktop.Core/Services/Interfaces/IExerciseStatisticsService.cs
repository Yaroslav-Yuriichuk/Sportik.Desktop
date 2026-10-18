using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Statistics;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IExerciseStatisticsService
    {
        Task<OperationResult<IEnumerable<WeekStatistics>>> GetWeeklyAsync(CancellationToken cancellationToken = default);

        Task<OperationResult<IEnumerable<MonthStatistics>>> GetMonthlyAsync(CancellationToken cancellationToken = default);

        Task<OperationResult<IEnumerable<YearStatistics>>> GetYearlyAsync(CancellationToken cancellationToken = default);

        Task<OperationResult<IEnumerable<AggregatedRepetitionsExerciseStatistics>>> GetAggregatedExerciseRepetitionsAsync(CancellationToken cancellationToken = default);

        Task<OperationResult<IEnumerable<AggregatedSetsExerciseStatistics>>> GetAggregatedExerciseSetsAsync(CancellationToken cancellationToken = default);

        Task<OperationResult<IEnumerable<AggregatedSetsDayStatistics>>> GetAggregatedDaySetsAsync(CancellationToken cancellationToken = default);

        Task<OperationResult<ExerciseSet>> AddSetAsync(AddExerciseSetModel addModel, CancellationToken cancellationToken = default);
    }
}
