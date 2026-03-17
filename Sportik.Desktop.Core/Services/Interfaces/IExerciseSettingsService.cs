using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Models.Settings;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IExerciseSettingsService
    {
        Task<OperationResult<ExerciseSettings>> UpdateAsync(ExerciseSettingsDelta delta, Guid exerciseId,
            CancellationToken cancellationToken = default);
    }
}
