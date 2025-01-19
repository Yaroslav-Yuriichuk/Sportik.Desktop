using System.Threading;
using System.Threading.Tasks;
using Sportik.Models;

namespace Sportik.Services.Exercises
{
    internal interface IExercisesRepository : ISyncRepository<Exercise>, IAsyncRepository<Exercise>
    {
        Exercise GetByKind(ExerciseKind exerciseKind);

        Task<Exercise> GetByKindAsync(ExerciseKind exerciseKind, CancellationToken cancellationToken = default);
    }
}
