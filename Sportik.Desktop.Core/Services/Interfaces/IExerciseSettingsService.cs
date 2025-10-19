using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models.Settings;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IExerciseSettingsService
    {
        Task<ExerciseSettings> UpdateAsync(ExerciseSettingsDelta exerciseSettingsDelta, Guid exerciseId, CancellationToken cancellationToken = default);
    }
}
