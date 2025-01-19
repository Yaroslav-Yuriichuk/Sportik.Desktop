using Sportik.Models;
using Sportik.Models.Settings;
using System.Threading.Tasks;
using System.Threading;

namespace Sportik.Services.Settings
{
    internal interface IExerciseSettingsRepository : ISyncRepository<ExerciseSettings>, IAsyncRepository<ExerciseSettings>
    {
        ExerciseSettings GetByKind(ExerciseKind exerciseKind);

        Task<ExerciseSettings> GetByKindAsync(ExerciseKind exerciseKind, CancellationToken cancellationToken = default);
    }
}
