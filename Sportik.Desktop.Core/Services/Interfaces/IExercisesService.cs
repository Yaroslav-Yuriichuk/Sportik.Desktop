using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Backend.Domain.Common;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IExercisesService
    {
        Task<OperationResult<IEnumerable<Exercise>>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<OperationResult<Exercise>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<OperationResult<IEnumerable<Exercise>>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    }
}
