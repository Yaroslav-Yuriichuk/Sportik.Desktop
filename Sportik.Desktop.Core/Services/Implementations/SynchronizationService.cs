using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Common.Synchronization;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Services.Implementations
{
    internal sealed class SynchronizationService : ISynchronizationService
    {
        private readonly IRuntimeCacheService _runtimeCacheService;
        private readonly IPersistentCacheService _persistentCacheService;
        private readonly IEventsService _eventsService;
        private readonly IAuthService _authService;
        private readonly IExercisesRepository _remoteExercisesRepository;
        private readonly IExercisesRepository _localExercisesRepository;
        private readonly IExerciseSettingsRepository _localExerciseSettingsRepository;
        private readonly IExerciseStatisticsRepository _remoteExerciseStatisticsRepository;
        private readonly IExerciseStatisticsRepository _localExerciseStatisticsRepository;

        public SynchronizationService(IRuntimeCacheService runtimeCacheService,
            IPersistentCacheService persistentCacheService,
            IEventsService eventsService,
            IAuthService authService,
            Func<DataSource, IExercisesRepository> exercisesRepositoryFactory,
            Func<DataSource, IExerciseSettingsRepository> exerciseSettingsRepositoryFactory,
            Func<DataSource, IExerciseStatisticsRepository> exerciseStatisticsRepositoryFactory)
        {
            _runtimeCacheService = runtimeCacheService;
            _persistentCacheService = persistentCacheService;
            _eventsService = eventsService;
            _authService = authService;
            _remoteExercisesRepository = exercisesRepositoryFactory(DataSource.Remote);
            _localExercisesRepository = exercisesRepositoryFactory(DataSource.Local);
            _localExerciseSettingsRepository = exerciseSettingsRepositoryFactory(DataSource.Local);
            _remoteExerciseStatisticsRepository = exerciseStatisticsRepositoryFactory(DataSource.Remote);
            _localExerciseStatisticsRepository = exerciseStatisticsRepositoryFactory(DataSource.Local);
        }

        public async Task<OperationResult> SyncAsync(ISynchronizer synchronizer, CancellationToken cancellationToken = default)
        {
            if (!_runtimeCacheService.TryGet(out AppModeCache appModeCache) || appModeCache.IsOffline)
            {
                return OperationResult.Failure(new[] { "Cannot sync while in offline mode." });
            }

            OperationResult<Guid> userIdResult = await _authService.GetUserIdAsync(cancellationToken);

            if (!userIdResult.Succeeded)
            {
                return OperationResult.Failure(new[] { "Failed to retrieve user ID for synchronization." });
            }

            Guid userId = userIdResult.Value;

            if (_persistentCacheService.TryGet(out SyncedUserCache syncedUserCache) && syncedUserCache.LastSyncedUserId != userId)
            {
                try
                {
                    ISynchronizer deleteSynchronizer = new DeleteAllSynchronizer();

                    deleteSynchronizer.Initialize(
                        _remoteExercisesRepository,
                        _localExercisesRepository,
                        _localExerciseSettingsRepository,
                        _localExerciseSettingsRepository,
                        _remoteExerciseStatisticsRepository,
                        _localExerciseStatisticsRepository,
                        _eventsService);

                    await deleteSynchronizer.SyncAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception)
                {
                    return OperationResult.Failure(new[] { "An error occurred during synchronization." });
                }
            }

            _persistentCacheService.Set(new SyncedUserCache
            {
                LastSyncedUserId = userId,
            });

            try
            {
                synchronizer.Initialize(
                    _remoteExercisesRepository,
                    _localExercisesRepository,
                    _localExerciseSettingsRepository,
                    _localExerciseSettingsRepository,
                    _remoteExerciseStatisticsRepository,
                    _localExerciseStatisticsRepository,
                    _eventsService);

                await synchronizer.SyncAsync(cancellationToken);

                return OperationResult.Success();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult.Failure(new[] { "An error occurred during synchronization." });
            }
        }
    }
}