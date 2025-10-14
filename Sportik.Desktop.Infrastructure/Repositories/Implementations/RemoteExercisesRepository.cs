using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Repositories.Interfaces;

namespace Sportik.Desktop.Infrastructure.Repositories.Implementations
{
    internal sealed class RemoteExercisesRepository : IExercisesRepository
    {
        public Task<Exercise> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Exercise>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<Exercise> AddAsync(Exercise entity, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<Exercise> UpdateAsync(Exercise entity, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<Exercise> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<Exercise> DeleteAsync(Exercise entity, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Exercise>> GetByIdsAsync(IEnumerable<int> exercisesIds, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}