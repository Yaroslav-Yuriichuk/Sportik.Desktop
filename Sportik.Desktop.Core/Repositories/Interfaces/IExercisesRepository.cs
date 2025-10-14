using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Repositories.Generic;

namespace Sportik.Desktop.Core.Repositories.Interfaces
{
    public interface IExercisesRepository : IAsyncRepository<Exercise>
    {
        Task<IEnumerable<Exercise>> GetByIdsAsync(IEnumerable<int> exercisesIds, CancellationToken cancellationToken = default);
    }
}
