using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Services.Implementations
{
    internal sealed class ExerciseSettingsService : IExerciseSettingsService
    {
        private readonly IExerciseSettingsRepository _remoteExerciseSettingsRepository;
        private readonly IExerciseSettingsRepository _localExerciseSettingsRepository;
        private readonly IRuntimeCacheService _runtimeCacheService;
        private readonly IEventsService _eventsService;

        private IExerciseSettingsRepository ExerciseSettingsRepository
        {
            get
            {
                if (!_runtimeCacheService.TryGet(out AppModeCache appModeCache))
                {
                    return _localExerciseSettingsRepository;
                }

                return appModeCache.IsOffline ? _localExerciseSettingsRepository : _remoteExerciseSettingsRepository;
            }
        }

        public ExerciseSettingsService(Func<DataSource, IExerciseSettingsRepository> exerciseSettingsRepositoryFactory,
            IRuntimeCacheService runtimeCacheService, IEventsService eventsService)
        {
            _remoteExerciseSettingsRepository = exerciseSettingsRepositoryFactory(DataSource.Remote);
            _localExerciseSettingsRepository = exerciseSettingsRepositoryFactory(DataSource.Local);
            _runtimeCacheService = runtimeCacheService;
            _eventsService = eventsService;
        }

        public async Task<OperationResult<ExerciseSettings>> UpdateAsync(ExerciseSettingsDelta delta, Guid exerciseId,
            CancellationToken cancellationToken)
        {
            try
            {
                ExerciseSettings updatedSettings =
                    await ExerciseSettingsRepository.UpdateAsync(delta, exerciseId, cancellationToken);

                ExerciseSettingsChange change = delta.Change;

                if (change.HasFlag(ExerciseSettingsChange.IsEnabled))
                {
                    _eventsService.RaiseEvent(new ExerciseIsEnabledChangedEventArgs(exerciseId, updatedSettings.IsEnabled));
                }

                if (change.HasFlag(ExerciseSettingsChange.TimeBetweenSets))
                {
                    _eventsService.RaiseEvent(new ExerciseTimeBetweenSetsChangedEventArgs(exerciseId, updatedSettings.TimeBetweenSets));
                }

                if (change.HasFlag(ExerciseSettingsChange.ExecutionTime))
                {
                    _eventsService.RaiseEvent(new ExerciseExecutionTimeChangedEventArgs(exerciseId, updatedSettings.ExecutionTime));
                }

                return OperationResult<ExerciseSettings>.Success(updatedSettings);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult<ExerciseSettings>.Failure(new[] { "Failed to update exercise settings." });
            }
        }
    }
}
