using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Common.Export;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Services.Implementations
{
    internal sealed class StatisticsExportService : IStatisticsExportService
    {
        private readonly IRuntimeCacheService _runtimeCacheService;
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

                return appModeCache.IsGuest ? _localExercisesRepository : _remoteExercisesRepository;
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

                return appModeCache.IsGuest ? _localExerciseStatisticsRepository : _remoteExerciseStatisticsRepository;
            }
        }

        public StatisticsExportService(IRuntimeCacheService runtimeCacheService,
            Func<DataSource, IExercisesRepository> exercisesRepositoryFactory,
            Func<DataSource, IExerciseStatisticsRepository> exerciseStatisticsRepositoryFactory)
        {
            _runtimeCacheService = runtimeCacheService;
            _remoteExercisesRepository = exercisesRepositoryFactory(DataSource.Remote);
            _localExercisesRepository = exercisesRepositoryFactory(DataSource.Local);
            _remoteExerciseStatisticsRepository = exerciseStatisticsRepositoryFactory(DataSource.Remote);
            _localExerciseStatisticsRepository = exerciseStatisticsRepositoryFactory(DataSource.Local);
        }

        public async Task<OperationResult> ExportAsync(IStatisticsExporter exporter, CancellationToken cancellationToken = default)
        {
            try
            {
                exporter.Initialize(ExercisesRepository, ExerciseStatisticsRepository);
                await exporter.ExportAsync(cancellationToken);

                return OperationResult.Success();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return OperationResult.Failure(new[] { $"Export failed: {ex.Message}", });
            }
        }
    }
}
