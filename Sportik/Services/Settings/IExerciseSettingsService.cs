using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Models;
using Sportik.Models.Settings;

namespace Sportik.Services.Settings
{
    internal interface IExerciseSettingsService
    {
        Task<IEnumerable<ExerciseSettings>> GetExerciseSettingsAsync(CancellationToken cancellationToken = default);

        Task<ExerciseSettings> GetExerciseSettingsAsync(Exercise exercise, CancellationToken cancellationToken = default);

        Task<ExerciseSettings> UpdateExerciseSettingsAsync(ExerciseSettingsDelta exerciseSettingsDelta, Exercise exercise, CancellationToken cancellationToken = default);
    }
}
