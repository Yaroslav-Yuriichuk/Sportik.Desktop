using Sportik.Models.Statistics;

namespace Sportik.Services.Statistics
{
    internal interface IExerciseStatisticsRepository : ISyncRepository<ExerciseStatistics>, IAsyncRepository<ExerciseStatistics> { }
}
