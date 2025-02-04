using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Core.Models.Statistics;
using Sportik.Core.Repositories.Generic;

namespace Sportik.Core.Repositories.Interfaces
{
    public interface IDayStatisticsRepository : ISyncRepository<DayStatistics>, IAsyncRepository<DayStatistics>
    {
        DayStatistics GetByDate(DateTime date);

        Task<DayStatistics> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default);
    }
}
