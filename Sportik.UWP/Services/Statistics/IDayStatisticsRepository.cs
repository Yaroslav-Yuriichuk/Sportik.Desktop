using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.UWP.Models.Statistics;

namespace Sportik.UWP.Services.Statistics
{
    internal interface IDayStatisticsRepository : ISyncRepository<DayStatistics>, IAsyncRepository<DayStatistics>
    {
        DayStatistics GetByDate(DateTime date);

        Task<DayStatistics> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default);
    }
}
