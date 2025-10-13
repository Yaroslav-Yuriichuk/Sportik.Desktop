using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Core.Repositories.Generic;

namespace Sportik.Desktop.Core.Repositories.Interfaces
{
    public interface IDayStatisticsRepository : IAsyncRepository<DayStatistics>
    {
        Task<DayStatistics> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default);
    }
}
