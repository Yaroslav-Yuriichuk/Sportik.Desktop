using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<OperationResult> SyncAsync(SyncOption option, CancellationToken cancellationToken = default)
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
                await _localExercisesRepository.DeleteAllAsync(cancellationToken);
            }

            _persistentCacheService.Set(new SyncedUserCache
            {
                LastSyncedUserId = userId,
            });

            if (option.HasFlag(SyncOption.Exercises))
            {
                OperationResult exercisesSyncResult = await SyncExercisesAsync(cancellationToken);

                if (!exercisesSyncResult.Succeeded)
                {
                    return exercisesSyncResult;
                }
            }

            if (option.HasFlag(SyncOption.ExerciseSettings))
            {
                OperationResult settingsSyncResult = await SyncExerciseSettingsAsync(cancellationToken);

                if (!settingsSyncResult.Succeeded)
                {
                    return settingsSyncResult;
                }
            }

            if (option.HasFlag(SyncOption.ExerciseStatistics))
            {
                OperationResult statisticsSyncResult = await SyncExerciseStatisticsAsync(cancellationToken);

                if (!statisticsSyncResult.Succeeded)
                {
                    return statisticsSyncResult;
                }
            }

            return OperationResult.Success();
        }

        private async Task<OperationResult> SyncExercisesAsync(CancellationToken cancellationToken)
        {
            try
            {
                Task<IEnumerable<Exercise>> remoteExercisesTask = _remoteExercisesRepository.GetAllAsync(cancellationToken);
                Task<IEnumerable<Exercise>> localExercisesTask = _localExercisesRepository.GetAllAsync(cancellationToken);

                await Task.WhenAll(remoteExercisesTask, localExercisesTask);

                IEnumerable<Exercise> remoteExercises = remoteExercisesTask.Result.ToList();
                IEnumerable<Exercise> localExercises = localExercisesTask.Result.ToList();

                HashSet<Guid> remoteExerciseIds = remoteExercises.Select(e => e.Id).ToHashSet();

                List<AddExerciseModel> localExercisesToAdd = localExercises
                    .Where(e => !remoteExerciseIds.Contains(e.Id))
                    .Select(e => new AddExerciseModel(e.Id, e.Name, e.Settings))
                    .ToList();

                Task<IEnumerable<Exercise>> addLocalExercisesTask = _remoteExercisesRepository.AddRangeAsync(localExercisesToAdd, cancellationToken);

                HashSet<Guid> localExerciseIds = localExercises.Select(e => e.Id).ToHashSet();

                List<AddExerciseModel> remoteExercisesToAdd = remoteExercises
                    .Where(e => !localExerciseIds.Contains(e.Id))
                    .Select(e => new AddExerciseModel(e.Id, e.Name, e.Settings))
                    .ToList();

                Task<IEnumerable<Exercise>> addRemoteExercisesTask = _localExercisesRepository.AddRangeAsync(remoteExercisesToAdd, cancellationToken);

                await Task.WhenAll(addLocalExercisesTask, addRemoteExercisesTask);

                foreach (Exercise exercise in addLocalExercisesTask.Result)
                {
                    _eventsService.RaiseEvent(new ExerciseCreatedEventArgs(exercise, false));
                }

                return OperationResult.Success();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult.Failure(new[] { "Failed to sync exercises." });
            }
        }

        private async Task<OperationResult> SyncExerciseSettingsAsync(CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<Exercise> remoteExercises = await _remoteExercisesRepository.GetAllAsync(cancellationToken);

                List<UpdateExerciseSettingsModel> updateModels = remoteExercises.Select(e =>
                    {
                        ExerciseSettingsDelta delta = new ExerciseSettingsDelta
                        {
                            Change = ExerciseSettingsChange.IsEnabled | ExerciseSettingsChange.TargetRepetitions |
                                     ExerciseSettingsChange.TimeBetweenSets | ExerciseSettingsChange.ExecutionTime,
                            IsEnabled = e.Settings.IsEnabled,
                            TargetRepetitions = e.Settings.TargetRepetitions,
                            TimeBetweenSets = e.Settings.TimeBetweenSets,
                            ExecutionTime = e.Settings.ExecutionTime
                        };

                        return new UpdateExerciseSettingsModel(e.Id, delta);
                    })
                    .ToList();

                await _localExerciseSettingsRepository.UpdateRangeAsync(updateModels, cancellationToken);

                return OperationResult.Success();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult.Failure(new[] { "Failed to sync exercise settings." });
            }
        }

        private async Task<OperationResult> SyncExerciseStatisticsAsync(CancellationToken cancellationToken)
        {
            try
            {
                Task<IEnumerable<ExerciseSet>> remoteSetsTask = _remoteExerciseStatisticsRepository.GetAllAsync(cancellationToken);
                Task<IEnumerable<ExerciseSet>> localSetsTask = _localExerciseStatisticsRepository.GetAllAsync(cancellationToken);

                await Task.WhenAll(remoteSetsTask, localSetsTask);

                IEnumerable<ExerciseSet> remoteSets = remoteSetsTask.Result.ToList();
                IEnumerable<ExerciseSet> localSets = localSetsTask.Result.ToList();

                HashSet<Guid> remoteSetIds = remoteSets.Select(set => set.Id).ToHashSet();

                List<AddExerciseSetModel> localSetsToAdd = localSets
                    .Where(set => !remoteSetIds.Contains(set.Id))
                    .Select(set => new AddExerciseSetModel(set.Id, set.Repetitions, set.LoggedAt, set.ExerciseId))
                    .ToList();

                Task<IEnumerable<ExerciseSet>> addLocalSetsTask = _remoteExerciseStatisticsRepository.AddRangeAsync(localSetsToAdd, cancellationToken);

                HashSet<Guid> localSetIds = localSets.Select(set => set.Id).ToHashSet();

                List<AddExerciseSetModel> remoteSetsToAdd = remoteSets
                    .Where(set => !localSetIds.Contains(set.Id))
                    .Select(set => new AddExerciseSetModel(set.Id, set.Repetitions, set.LoggedAt, set.ExerciseId))
                    .ToList();

                Task<IEnumerable<ExerciseSet>> addRemoteSetsTask = _localExerciseStatisticsRepository.AddRangeAsync(remoteSetsToAdd, cancellationToken);

                await Task.WhenAll(addLocalSetsTask, addRemoteSetsTask);

                foreach (ExerciseSet exerciseSet in addLocalSetsTask.Result)
                {
                    _eventsService.RaiseEvent(new ExerciseSetAddedEventArgs(exerciseSet, false));
                }

                return OperationResult.Success();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult.Failure(new[] { "An error occurred during synchronization of exercise statistics." });
            }
        }
    }
}