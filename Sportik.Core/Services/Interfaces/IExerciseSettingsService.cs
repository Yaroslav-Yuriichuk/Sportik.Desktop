using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Core.Models;
using Sportik.Core.Models.Settings;

namespace Sportik.Core.Services.Interfaces
{
    public interface IExerciseSettingsService
    {
        Task<IEnumerable<ExerciseSettings>> GetAllExerciseSettingsAsync(CancellationToken cancellationToken = default);

        ExerciseSettings GetExerciseSettings(Exercise exercise);

        Task<ExerciseSettings> GetExerciseSettingsAsync(Exercise exercise, CancellationToken cancellationToken = default);

        Task<ExerciseSettings> UpdateExerciseSettingsAsync(ExerciseSettingsDelta exerciseSettingsDelta, Exercise exercise, CancellationToken cancellationToken = default);
    }
}
