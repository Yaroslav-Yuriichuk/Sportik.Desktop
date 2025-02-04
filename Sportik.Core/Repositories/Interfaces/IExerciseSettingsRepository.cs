using System.Threading.Tasks;
using System.Threading;
using Sportik.Core.Models;
using Sportik.Core.Models.Settings;
using Sportik.Core.Repositories.Generic;

namespace Sportik.Core.Repositories.Interfaces
{
    public interface IExerciseSettingsRepository : ISyncRepository<ExerciseSettings>, IAsyncRepository<ExerciseSettings>
    {
        ExerciseSettings GetByKind(ExerciseKind exerciseKind);

        Task<ExerciseSettings> GetByKindAsync(ExerciseKind exerciseKind, CancellationToken cancellationToken = default);
    }
}
