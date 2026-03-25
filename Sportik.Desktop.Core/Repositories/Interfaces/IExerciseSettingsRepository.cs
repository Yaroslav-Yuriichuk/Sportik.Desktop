using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Repositories.Interfaces
{
    public interface IExerciseSettingsRepository
    {
        Task<Exercise> UpdateAsync(UpdateExerciseSettingsModel updateModel, CancellationToken cancellationToken = default);

        Task<IEnumerable<Exercise>> UpdateRangeAsync(IEnumerable<UpdateExerciseSettingsModel> updateModels,
            CancellationToken cancellationToken = default);
    }
}
