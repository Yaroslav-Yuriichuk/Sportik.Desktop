using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.UWP.Models;
using Sportik.UWP.Models.Settings;

namespace Sportik.UWP.Services.Settings
{
    internal interface IExerciseSettingsService
    {
        Task<IEnumerable<ExerciseSettings>> GetAllExerciseSettingsAsync(CancellationToken cancellationToken = default);

        ExerciseSettings GetExerciseSettings(Exercise exercise);

        Task<ExerciseSettings> GetExerciseSettingsAsync(Exercise exercise, CancellationToken cancellationToken = default);

        Task<ExerciseSettings> UpdateExerciseSettingsAsync(ExerciseSettingsDelta exerciseSettingsDelta, Exercise exercise, CancellationToken cancellationToken = default);
    }
}
