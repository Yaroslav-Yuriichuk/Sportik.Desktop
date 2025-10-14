using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Core.Repositories.Interfaces;

namespace Sportik.Desktop.Infrastructure.Repositories.Implementations
{
    internal sealed class RemoteExerciseStatisticsRepository : IExerciseStatisticsRepository
    {
        public Task<ExerciseStatistics> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<ExerciseStatistics>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<ExerciseStatistics> AddAsync(ExerciseStatistics entity, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<ExerciseStatistics> UpdateAsync(ExerciseStatistics entity, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<ExerciseStatistics> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<ExerciseStatistics> DeleteAsync(ExerciseStatistics entity, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}