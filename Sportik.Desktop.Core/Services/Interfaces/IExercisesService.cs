using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IExercisesService
    {
        IEnumerable<Exercise> GetAllExercises();

        Task<IEnumerable<Exercise>> GetAllExercisesAsync(CancellationToken cancellationToken = default);

        IEnumerable<Exercise> GetExercises(IEnumerable<int> exercisesIds);
    }
}
