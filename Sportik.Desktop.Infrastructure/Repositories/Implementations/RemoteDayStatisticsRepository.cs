using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Core.Repositories.Interfaces;

namespace Sportik.Desktop.Infrastructure.Repositories.Implementations
{
    internal sealed class RemoteDayStatisticsRepository : IDayStatisticsRepository
    {
        public Task<DayStatistics> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DayStatistics>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<DayStatistics> AddAsync(DayStatistics entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<DayStatistics> UpdateAsync(DayStatistics entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<DayStatistics> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<DayStatistics> DeleteAsync(DayStatistics entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<DayStatistics> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}