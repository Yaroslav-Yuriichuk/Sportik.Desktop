using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IExercisesService
    {
        Task<IEnumerable<Exercise>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Exercise> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Exercise>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    }
}
