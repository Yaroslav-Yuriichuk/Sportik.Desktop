using Sportik.UWP.Models.Statistics;

namespace Sportik.UWP.Services.Statistics
{
    internal interface IExerciseStatisticsRepository : ISyncRepository<ExerciseStatistics>, IAsyncRepository<ExerciseStatistics> { }
}
