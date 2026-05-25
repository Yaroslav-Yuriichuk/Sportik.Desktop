using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Common.Import;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Services.Implementations
{
    internal sealed class StatisticsImportService : IStatisticsImportService
    {
        private readonly IRuntimeCacheService _runtimeCacheService;
        private readonly IEventsService _eventsService;
        private readonly IExercisesRepository _remoteExercisesRepository;
        private readonly IExercisesRepository _localExercisesRepository;
        private readonly IExerciseStatisticsRepository _remoteExerciseStatisticsRepository;
        private readonly IExerciseStatisticsRepository _localExerciseStatisticsRepository;

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

        public StatisticsImportService(IRuntimeCacheService runtimeCacheService,
            IEventsService eventsService,
            Func<DataSource, IExercisesRepository> exercisesRepositoryFactory,
            Func<DataSource, IExerciseStatisticsRepository> exerciseStatisticsRepositoryFactory)
        {
            _runtimeCacheService = runtimeCacheService;
            _eventsService = eventsService;
            _remoteExercisesRepository = exercisesRepositoryFactory(DataSource.Remote);
            _localExercisesRepository = exercisesRepositoryFactory(DataSource.Local);
            _remoteExerciseStatisticsRepository = exerciseStatisticsRepositoryFactory(DataSource.Remote);
            _localExerciseStatisticsRepository = exerciseStatisticsRepositoryFactory(DataSource.Local);
        }

        public async Task<OperationResult> ImportAsync(IStatisticsImporter importer, CancellationToken cancellationToken = default)
        {
            try
            {
                importer.Initialize(ExercisesRepository, ExerciseStatisticsRepository, _eventsService);

                await importer.ImportAsync(cancellationToken);

                return OperationResult.Success();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return OperationResult.Failure(new[] { $"Import failed: {ex.Message}", });
            }
        }
    }
}