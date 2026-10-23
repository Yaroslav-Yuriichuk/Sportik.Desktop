using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Common.DataExchange;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Services.Implementations
{
    internal sealed class DataExchangeService : IDataExchangeService
    {
        private readonly IRuntimeCacheService _runtimeCacheService;
        private readonly IEventsService _eventsService;
        private readonly Func<DataSource, IExercisesRepository> _exercisesRepositoryFactory;
        private readonly Func<DataSource, IExerciseStatisticsRepository> _exerciseStatisticsRepositoryFactory;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private IExercisesRepository ExercisesRepository
        {
            get
            {
                if (!_runtimeCacheService.TryGet(out AppModeCache appModeCache))
                {
                    return _exercisesRepositoryFactory(DataSource.Local);
                }

                return _exercisesRepositoryFactory(appModeCache.IsGuest ? DataSource.Local : DataSource.Remote);
            }
        }

        private IExerciseStatisticsRepository ExerciseStatisticsRepository
        {
            get
            {
                if (!_runtimeCacheService.TryGet(out AppModeCache appModeCache))
                {
                    return _exerciseStatisticsRepositoryFactory(DataSource.Local);
                }

                return _exerciseStatisticsRepositoryFactory(appModeCache.IsGuest ? DataSource.Local : DataSource.Remote);
            }
        }

        public DataExchangeService(
            IRuntimeCacheService runtimeCacheService,
            IEventsService eventsService,
            Func<DataSource, IExercisesRepository> exercisesRepositoryFactory,
            Func<DataSource, IExerciseStatisticsRepository> exerciseStatisticsRepositoryFactory)
        {
            _runtimeCacheService = runtimeCacheService;
            _eventsService = eventsService;
            _exercisesRepositoryFactory = exercisesRepositoryFactory;
            _exerciseStatisticsRepositoryFactory = exerciseStatisticsRepositoryFactory;
        }

        public async Task<OperationResult> ExchangeAsync(IDataExchanger exchanger, DataExchangeContext context = null, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                exchanger.Initialize(ExercisesRepository, ExerciseStatisticsRepository, _eventsService);
                await exchanger.ExchangeAsync(context, cancellationToken);

                return OperationResult.Success();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return OperationResult.Failure(new[] { $"Data exchange failed: {ex.Message}", });
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}