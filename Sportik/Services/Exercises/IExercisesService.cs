using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Models;

namespace Sportik.Services.Exercises
{
    internal interface IExercisesService
    {
        Task<IEnumerable<Exercise>> GetExercisesAsync(CancellationToken cancellationToken = default);
    }
}
