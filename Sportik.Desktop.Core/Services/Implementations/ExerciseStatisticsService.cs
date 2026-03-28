using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Services.Implementations
{
    internal sealed class ExerciseStatisticsService : IExerciseStatisticsService
    {
        private readonly IExerciseStatisticsRepository _remoteExerciseStatisticsRepository;
        private readonly IExerciseStatisticsRepository _localExerciseStatisticsRepository;
        private readonly IRuntimeCacheService _runtimeCacheService;

        private IExerciseStatisticsRepository ExerciseStatisticsRepository
        {
            get
            {
                if (!_runtimeCacheService.TryGet(out AppModeCache appModeCache))
                {
                    return _localExerciseStatisticsRepository;
                }

                return appModeCache.IsOffline ? _localExerciseStatisticsRepository : _remoteExerciseStatisticsRepository;
            }
        }

        public ExerciseStatisticsService(Func<DataSource, IExerciseStatisticsRepository> exerciseStatisticsRepositoryFactory,
             IRuntimeCacheService runtimeCacheService)
        {
            _remoteExerciseStatisticsRepository = exerciseStatisticsRepositoryFactory(DataSource.Remote);
            _localExerciseStatisticsRepository = exerciseStatisticsRepositoryFactory(DataSource.Local);
            _runtimeCacheService = runtimeCacheService;
        }

        public async Task<OperationResult<IEnumerable<WeekStatistics>>> GetWeeklyAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<WeekStatistics> weekStatistics = await ExerciseStatisticsRepository.GetWeeklyAsync(cancellationToken);
                return OperationResult<IEnumerable<WeekStatistics>>.Success(weekStatistics);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult<IEnumerable<WeekStatistics>>.Failure(new[] { "Failed to retrieve weekly exercise statistics." });
            }
        }

        public async Task<OperationResult<ExerciseSet>> AddSetAsync(AddExerciseSetModel addModel, CancellationToken cancellationToken = default)
        {
            try
            {
                ExerciseSet addedSet = await ExerciseStatisticsRepository.AddSetAsync(addModel, cancellationToken);
                return OperationResult<ExerciseSet>.Success(addedSet);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult<ExerciseSet>.Failure(new[] { "Failed to add the exercise set." });
            }
        }

        public async Task<OperationResult> SyncAsync(CancellationToken cancellationToken = default)
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
