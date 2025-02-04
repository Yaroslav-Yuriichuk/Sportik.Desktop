using System.Threading;
using System.Threading.Tasks;
using Sportik.Core.Models;
using Sportik.Core.Repositories.Generic;

namespace Sportik.Core.Repositories.Interfaces
{
    public interface IExercisesRepository : ISyncRepository<Exercise>, IAsyncRepository<Exercise>
    {
        Exercise GetByKind(ExerciseKind exerciseKind);

        Task<Exercise> GetByKindAsync(ExerciseKind exerciseKind, CancellationToken cancellationToken = default);
    }
}
