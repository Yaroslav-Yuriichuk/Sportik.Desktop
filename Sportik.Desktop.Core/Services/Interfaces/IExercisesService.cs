using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IExercisesService
    {
        Task<OperationResult<IEnumerable<Exercise>>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<OperationResult<Exercise>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<OperationResult<IEnumerable<Exercise>>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

        Task<OperationResult<Exercise>> AddAsync(string name, ExerciseSettings settings, CancellationToken cancellationToken = default);
    }
}
