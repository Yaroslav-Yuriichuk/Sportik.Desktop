using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IExerciseSettingsService
    {
        Task<OperationResult<Exercise>> UpdateAsync(UpdateExerciseSettingsModel updateModel, CancellationToken cancellationToken = default);

        Task<OperationResult<IEnumerable<Exercise>>> UpdateRangeAsync(IEnumerable<UpdateExerciseSettingsModel> updateModels,
            CancellationToken cancellationToken = default);

        Task<OperationResult> SyncAsync(CancellationToken cancellationToken = default);
    }
}
