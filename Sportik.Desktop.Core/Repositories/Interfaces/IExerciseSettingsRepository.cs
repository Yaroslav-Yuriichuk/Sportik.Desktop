using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models.Settings;

namespace Sportik.Desktop.Core.Repositories.Interfaces
{
    public interface IExerciseSettingsRepository
    {
        Task<ExerciseSettings> UpdateAsync(ExerciseSettingsDelta delta, Guid exerciseId, CancellationToken cancellationToken = default);
    }
}
