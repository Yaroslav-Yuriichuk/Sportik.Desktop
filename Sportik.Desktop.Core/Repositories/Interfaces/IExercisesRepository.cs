using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;

namespace Sportik.Desktop.Core.Repositories.Interfaces
{
    public interface IExercisesRepository
    {
        Task<IEnumerable<Exercise>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Exercise> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Exercise>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

        Task<Exercise> AddAsync(string name, ExerciseSettings settings, CancellationToken cancellationToken = default);
    }
}
