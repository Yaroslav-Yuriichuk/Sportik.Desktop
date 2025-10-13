using System.Threading.Tasks;
using System.Threading;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Core.Repositories.Generic;

namespace Sportik.Desktop.Core.Repositories.Interfaces
{
    public interface IExerciseSettingsRepository : IAsyncRepository<ExerciseSettings>
    {
        Task<ExerciseSettings> GetByKindAsync(ExerciseKind exerciseKind, CancellationToken cancellationToken = default);
    }
}
