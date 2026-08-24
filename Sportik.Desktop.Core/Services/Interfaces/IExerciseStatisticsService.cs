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

        Task<OperationResult<IEnumerable<AggregatedRepetitionsExerciseStatistics>>> GetAggregatedRepetitionsAsync(CancellationToken cancellationToken = default);

        Task<OperationResult<IEnumerable<AggregatedSetsExerciseStatistics>>> GetAggregatedSetsAsync(CancellationToken cancellationToken = default);

        Task<OperationResult<ExerciseSet>> AddSetAsync(AddExerciseSetModel addModel, CancellationToken cancellationToken = default);
    }
}
