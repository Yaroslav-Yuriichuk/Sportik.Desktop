using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Common.Synchronization
{
    internal interface ISynchronizer
    {
        void Initialize(
            IExercisesRepository remoteExercisesRepository,
            IExercisesRepository localExercisesRepository,
            IExerciseSettingsRepository remoteExerciseSettingsRepository,
            IExerciseSettingsRepository localExerciseSettingsRepository,
            IExerciseStatisticsRepository remoteExerciseStatisticsRepository,
            IExerciseStatisticsRepository localExerciseStatisticsRepository,
            IEventsService eventsService);

        Task SyncAsync(CancellationToken cancellationToken);
    }
}