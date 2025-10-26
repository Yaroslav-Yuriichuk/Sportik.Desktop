using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Backend.Domain.Common;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Statistics;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IExerciseStatisticsService
    {
        Task<OperationResult<IEnumerable<WeekStatistics>>> GetWeeklyAsync(CancellationToken cancellationToken = default);

        Task<OperationResult<ExerciseSet>> AddSetAsync(ExerciseSet set, Guid exerciseId, CancellationToken cancellationToken = default);
    }
}
