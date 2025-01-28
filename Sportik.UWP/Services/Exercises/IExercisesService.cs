using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.UWP.Models;

namespace Sportik.UWP.Services.Exercises
{
    internal interface IExercisesService
    {
        IEnumerable<Exercise> GetAllExercises();

        Task<IEnumerable<Exercise>> GetAllExercisesAsync(CancellationToken cancellationToken = default);
    }
}
