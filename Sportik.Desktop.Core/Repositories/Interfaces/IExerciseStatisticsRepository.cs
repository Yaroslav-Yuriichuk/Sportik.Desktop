using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Statistics;

namespace Sportik.Desktop.Core.Repositories.Interfaces
{
    public interface IExerciseStatisticsRepository
    {
        Task<IEnumerable<WeekStatistics>> GetWeeklyAsync(CancellationToken cancellationToken = default);

        Task<ExerciseSet> AddSetAsync(AddExerciseSetModel addModel, Guid exerciseId, CancellationToken cancellationToken = default);
    }
}
