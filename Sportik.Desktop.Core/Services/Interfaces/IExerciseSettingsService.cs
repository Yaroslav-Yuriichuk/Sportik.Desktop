using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IExerciseSettingsService
    {
        Task<IEnumerable<ExerciseSettings>> GetAllExerciseSettingsAsync(CancellationToken cancellationToken = default);

        Task<ExerciseSettings> GetExerciseSettingsAsync(Exercise exercise, CancellationToken cancellationToken = default);

        Task<ExerciseSettings> UpdateExerciseSettingsAsync(ExerciseSettingsDelta exerciseSettingsDelta, Exercise exercise, CancellationToken cancellationToken = default);
    }
}
