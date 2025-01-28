using System.Threading.Tasks;
using System.Threading;
using Sportik.UWP.Models;
using Sportik.UWP.Models.Settings;

namespace Sportik.UWP.Services.Settings
{
    internal interface IExerciseSettingsRepository : ISyncRepository<ExerciseSettings>, IAsyncRepository<ExerciseSettings>
    {
        ExerciseSettings GetByKind(ExerciseKind exerciseKind);

        Task<ExerciseSettings> GetByKindAsync(ExerciseKind exerciseKind, CancellationToken cancellationToken = default);
    }
}
