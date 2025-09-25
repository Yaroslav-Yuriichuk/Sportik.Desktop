using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Repositories.Generic;

namespace Sportik.Desktop.Core.Repositories.Interfaces
{
    public interface IExercisesRepository : ISyncRepository<Exercise>, IAsyncRepository<Exercise>
    {
        Exercise GetByKind(ExerciseKind exerciseKind);

        Task<Exercise> GetByKindAsync(ExerciseKind exerciseKind, CancellationToken cancellationToken = default);
    }
}
