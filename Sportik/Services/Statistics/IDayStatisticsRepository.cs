using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Models.Statistics;

namespace Sportik.Services.Statistics
{
    internal interface IDayStatisticsRepository : IRepository<DayStatistics>
    {
        Task<DayStatistics> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default);
    }
}
