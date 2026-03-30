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
        private readonly IEventsService _eventsService;
        private readonly IExercisesRepository _remoteExercisesRepository;
        private readonly IExercisesRepository _localExercisesRepository;
        private readonly IExerciseSettingsRepository _remoteExerciseSettingsRepository;
        private readonly IExerciseStatisticsRepository _remoteExerciseStatisticsRepository;
        private readonly IExerciseStatisticsRepository _localExerciseStatisticsRepository;

        public SynchronizationService(IRuntimeCacheService runtimeCacheService,
            IEventsService eventsService,
            Func<DataSource, IExercisesRepository> exercisesRepositoryFactory,
            Func<DataSource, IExerciseSettingsRepository> exerciseSettingsRepositoryFactory,
            Func<DataSource, IExerciseStatisticsRepository> exerciseStatisticsRepositoryFactory)
        {
            _runtimeCacheService = runtimeCacheService;
            _eventsService = eventsService;
            _remoteExercisesRepository = exercisesRepositoryFactory(DataSource.Remote);
            _localExercisesRepository = exercisesRepositoryFactory(DataSource.Local);
            _remoteExerciseSettingsRepository = exerciseSettingsRepositoryFactory(DataSource.Remote);
            _remoteExerciseStatisticsRepository = exerciseStatisticsRepositoryFactory(DataSource.Remote);
            _localExerciseStatisticsRepository = exerciseStatisticsRepositoryFactory(DataSource.Local);
        }

        public async Task<OperationResult> SyncAsync(CancellationToken cancellationToken = default)
        {
            if (!_runtimeCacheService.TryGet(out AppModeCache appModeCache) || appModeCache.IsOffline)
            {
                return OperationResult.Failure(new[] { "Cannot sync while in offline mode." });
            }

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
                    _eventsService.RaiseEvent(new ExerciseCreatedEventArgs(exercise));
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult.Failure(new[] { "Failed to sync exercises." });
            }

            try
            {
                IEnumerable<Exercise> localExercises = await _localExercisesRepository.GetAllAsync(cancellationToken);

                List<UpdateExerciseSettingsModel> updateModels = localExercises.Select(e =>
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

                IEnumerable<Exercise> exercises = await _remoteExerciseSettingsRepository.UpdateRangeAsync(updateModels, cancellationToken);

                foreach (Exercise exercise in exercises)
                {
                    _eventsService.RaiseEvent(new ExerciseIsEnabledChangedEventArgs(exercise.Id, exercise.Settings.IsEnabled));
                    _eventsService.RaiseEvent(new ExerciseTimeBetweenSetsChangedEventArgs(exercise.Id, exercise.Settings.TimeBetweenSets));
                    _eventsService.RaiseEvent(new ExerciseExecutionTimeChangedEventArgs(exercise.Id, exercise.Settings.ExecutionTime));
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult.Failure(new[] { "Failed to sync exercise settings." });
            }

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
                    _eventsService.RaiseEvent(new ExerciseSetAddedEventArgs(exerciseSet));
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult.Failure(new[] { "An error occurred during synchronization of exercise statistics." });
            }

            return OperationResult.Success();
        }
    }
}