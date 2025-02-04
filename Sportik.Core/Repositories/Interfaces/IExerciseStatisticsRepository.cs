using Sportik.Core.Models.Statistics;
using Sportik.Core.Repositories.Generic;

namespace Sportik.Core.Repositories.Interfaces
{
    public interface IExerciseStatisticsRepository : ISyncRepository<ExerciseStatistics>, IAsyncRepository<ExerciseStatistics> { }
}
