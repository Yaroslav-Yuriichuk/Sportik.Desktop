using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Core.Repositories.Interfaces;

namespace Sportik.Desktop.Infrastructure.Repositories.Implementations
{
    internal sealed class RemoteExerciseSettingsRepository : IExerciseSettingsRepository
    {
        public Task<ExerciseSettings> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<ExerciseSettings>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<ExerciseSettings> AddAsync(ExerciseSettings entity, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<ExerciseSettings> UpdateAsync(ExerciseSettings entity, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<ExerciseSettings> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<ExerciseSettings> DeleteAsync(ExerciseSettings entity, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}