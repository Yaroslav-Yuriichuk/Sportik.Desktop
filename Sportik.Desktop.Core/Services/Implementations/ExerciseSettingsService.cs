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
    internal sealed class ExerciseSettingsService : IExerciseSettingsService
    {
        private readonly IExercisesRepository _localExercisesRepository;
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

        public ExerciseSettingsService(Func<DataSource, IExercisesRepository> exercisesRepositoryFactory,
            Func<DataSource, IExerciseSettingsRepository> exerciseSettingsRepositoryFactory,
            IRuntimeCacheService runtimeCacheService, IEventsService eventsService)
        {
            _localExercisesRepository = exercisesRepositoryFactory(DataSource.Local);
            _remoteExerciseSettingsRepository = exerciseSettingsRepositoryFactory(DataSource.Remote);
            _localExerciseSettingsRepository = exerciseSettingsRepositoryFactory(DataSource.Local);
            _runtimeCacheService = runtimeCacheService;
            _eventsService = eventsService;
        }

        public async Task<OperationResult<Exercise>> UpdateAsync(UpdateExerciseSettingsModel updateModel,
            CancellationToken cancellationToken)
        {
            try
            {
                Exercise updatedExercise = await ExerciseSettingsRepository.UpdateAsync(updateModel, cancellationToken);

                ExerciseSettingsDelta delta = updateModel.Delta;
                ExerciseSettingsChange change = delta.Change;

                if (change.HasFlag(ExerciseSettingsChange.IsEnabled))
                {
                    _eventsService.RaiseEvent(new ExerciseIsEnabledChangedEventArgs(updatedExercise.Id, updatedExercise.Settings.IsEnabled));
                }

                if (change.HasFlag(ExerciseSettingsChange.TargetRepetitions))
                {
                    _eventsService.RaiseEvent(new ExerciseTargetRepetitionsChangedEventArgs(updatedExercise.Id, updatedExercise.Settings.TargetRepetitions));
                }

                if (change.HasFlag(ExerciseSettingsChange.TimeBetweenSets))
                {
                    _eventsService.RaiseEvent(new ExerciseTimeBetweenSetsChangedEventArgs(updatedExercise.Id, updatedExercise.Settings.TimeBetweenSets));
                }

                if (change.HasFlag(ExerciseSettingsChange.ExecutionTime))
                {
                    _eventsService.RaiseEvent(new ExerciseExecutionTimeChangedEventArgs(updatedExercise.Id, updatedExercise.Settings.ExecutionTime));
                }

                return OperationResult<Exercise>.Success(updatedExercise);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult<Exercise>.Failure(new[] { "Failed to update exercise settings." });
            }
        }

        public async Task<OperationResult<IEnumerable<Exercise>>> UpdateRangeAsync(IEnumerable<UpdateExerciseSettingsModel> updateModels,
            CancellationToken cancellationToken = default)
        {
            try
            {
                updateModels = updateModels as IList<UpdateExerciseSettingsModel> ?? updateModels.ToList();

                IEnumerable<Exercise> updatedExercises = await ExerciseSettingsRepository.UpdateRangeAsync(updateModels, cancellationToken);
                updatedExercises = updatedExercises as IList<Exercise> ?? updatedExercises.ToList();

                Dictionary<Guid, UpdateExerciseSettingsModel> updateModelsById = updateModels.ToDictionary(m => m.ExerciseId);

                foreach (Exercise exercise in updatedExercises)
                {
                    if (!updateModelsById.TryGetValue(exercise.Id, out UpdateExerciseSettingsModel updateModel))
                    {
                        continue;
                    }

                    ExerciseSettingsDelta delta = updateModel.Delta;
                    ExerciseSettingsChange change = delta.Change;

                    if (change.HasFlag(ExerciseSettingsChange.IsEnabled))
                    {
                        _eventsService.RaiseEvent(new ExerciseIsEnabledChangedEventArgs(exercise.Id, exercise.Settings.IsEnabled));
                    }

                    if (change.HasFlag(ExerciseSettingsChange.TargetRepetitions))
                    {
                        _eventsService.RaiseEvent(new ExerciseTargetRepetitionsChangedEventArgs(exercise.Id, exercise.Settings.TargetRepetitions));
                    }

                    if (change.HasFlag(ExerciseSettingsChange.TimeBetweenSets))
                    {
                        _eventsService.RaiseEvent(new ExerciseTimeBetweenSetsChangedEventArgs(exercise.Id, exercise.Settings.TimeBetweenSets));
                    }

                    if (change.HasFlag(ExerciseSettingsChange.ExecutionTime))
                    {
                        _eventsService.RaiseEvent(new ExerciseExecutionTimeChangedEventArgs(exercise.Id, exercise.Settings.ExecutionTime));
                    }
                }

                return OperationResult<IEnumerable<Exercise>>.Success(updatedExercises);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult<IEnumerable<Exercise>>.Failure(new[] { "An unexpected error occurred while updating exercise settings." });
            }
        }
    }
}
