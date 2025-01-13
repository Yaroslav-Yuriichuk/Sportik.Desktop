using System.Threading;
using System.Threading.Tasks;
using Sportik.Models;

namespace Sportik.Services.Exercises
{
    internal interface IExercisesRepository : IRepository<Exercise>
    {
        Task<Exercise> GetByKindAsync(ExerciseKind exerciseKind, CancellationToken cancellationToken = default);
    }
}
