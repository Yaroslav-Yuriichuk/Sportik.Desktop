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

        Task<IEnumerable<ExerciseSet>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<ExerciseSet> AddSetAsync(AddExerciseSetModel addModel, CancellationToken cancellationToken = default);

        Task<IEnumerable<ExerciseSet>> AddRangeAsync(IEnumerable<AddExerciseSetModel> addModels, CancellationToken cancellationToken = default);
    }
}
