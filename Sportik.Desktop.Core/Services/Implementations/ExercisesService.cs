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
    internal sealed class ExercisesService : IExercisesService
    {
        private readonly IExercisesRepository _remoteExercisesRepository;
        private readonly IExercisesRepository _localExercisesRepository;
        private readonly IRuntimeCacheService _runtimeCacheService;
        private readonly IEventsService _eventsService;

        private IExercisesRepository ExercisesRepository
        {
            get
            {
                if (!_runtimeCacheService.TryGet(out AppModeCache appModeCache))
                {
                    return _localExercisesRepository;
                }

                return appModeCache.IsOffline ? _localExercisesRepository : _remoteExercisesRepository;
            }
        }

        public ExercisesService(Func<DataSource, IExercisesRepository> exercisesRepositoryFactory,
            IRuntimeCacheService runtimeCacheService, IEventsService eventsService)
        {
            _remoteExercisesRepository = exercisesRepositoryFactory(DataSource.Remote);
            _localExercisesRepository = exercisesRepositoryFactory(DataSource.Local);
            _runtimeCacheService = runtimeCacheService;
            _eventsService = eventsService;
        }

        public async Task<OperationResult<IEnumerable<Exercise>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<Exercise> exercises = await ExercisesRepository.GetAllAsync(cancellationToken);
                return OperationResult<IEnumerable<Exercise>>.Success(exercises);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult<IEnumerable<Exercise>>.Failure(new[] { "Failed to retrieve exercises." });
            }
        }

        public async Task<OperationResult<Exercise>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                Exercise exercise = await ExercisesRepository.GetByIdAsync(id, cancellationToken);
                return OperationResult<Exercise>.Success(exercise);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult<Exercise>.Failure(new[] { "Failed to retrieve exercise." });
            }
        }

        public async Task<OperationResult<IEnumerable<Exercise>>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<Exercise> exercises = await ExercisesRepository.GetByIdsAsync(ids, cancellationToken);
                return OperationResult<IEnumerable<Exercise>>.Success(exercises);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult<IEnumerable<Exercise>>.Failure(new[] { "Failed to retrieve exercises." });
            }
        }

        public async Task<OperationResult<Exercise>> AddAsync(string name, ExerciseSettings settings, CancellationToken cancellationToken = default)
        {
            try
            {
                Exercise exercise = await ExercisesRepository.AddAsync(new AddExerciseModel(null, name, settings), cancellationToken);
                _eventsService.RaiseEvent(new ExerciseCreatedEventArgs(exercise));

                return OperationResult<Exercise>.Success(exercise);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult<Exercise>.Failure(new[] { "Failed to add exercise." });
            }
        }

        public async Task<OperationResult> SyncAsync(CancellationToken cancellationToken = default)
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
    }
}
