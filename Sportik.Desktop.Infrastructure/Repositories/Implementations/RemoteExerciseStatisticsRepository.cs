using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Extensions;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Infrastructure.DTOs.Statistics;
using Sportik.Desktop.Infrastructure.Mappers.Statistics;
using Sportik.Desktop.Infrastructure.Models;
using Sportik.Desktop.Infrastructure.Services.Interfaces;

namespace Sportik.Desktop.Infrastructure.Repositories.Implementations
{
    internal sealed class RemoteExerciseStatisticsRepository : IExerciseStatisticsRepository
    {
        private readonly IApiService _apiService;
        private readonly IAuthService _authService;
        private readonly IPersistentCacheService _persistentCacheService;

        public RemoteExerciseStatisticsRepository(IApiService apiService, IAuthService authService,
            IPersistentCacheService persistentCacheService)
        {
            _apiService = apiService;
            _authService = authService;
            _persistentCacheService = persistentCacheService;
        }

        public async Task<IEnumerable<WeekStatistics>> GetWeeklyAsync(CancellationToken cancellationToken = default)
        {
            OperationResult<string> authResult = await _authService.GetTokenAsync(cancellationToken);

            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.UtcNow);

            IEnumerable<WeekStatisticsDto> weekStatistics = await _apiService.GetAsync<IEnumerable<WeekStatisticsDto>>(
                $"/api/ExerciseStatistics/weekly?order={WeekStatisticsOrder.Descending}&offset={offset}",
                authResult.Value,
                cancellationToken);

            EnabledExercisesCache enabledExercisesCache = _persistentCacheService.GetOrNew<EnabledExercisesCache>();

            return weekStatistics.Select(ws => WeekStatisticsMapper.ToDomain(ws, enabledExercisesCache.IncludesExercise));
        }

        public async Task<IEnumerable<AggregatedRepetitionsExerciseStatistics>> GetAggregatedExerciseRepetitionsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException("Aggregated repetitions exercise statistics are not supported in the remote repository.");
        }

        public async Task<IEnumerable<AggregatedSetsExerciseStatistics>> GetAggregatedExerciseSetsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException("Aggregated sets exercise statistics are not supported in the remote repository.");
        }

        public async Task<IEnumerable<AggregatedSetsDayStatistics>> GetAggregatedDaySetsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException("Aggregated day sets statistics are not supported in the remote repository.");
        }

        public async Task<IEnumerable<ExerciseSet>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            OperationResult<string> authResult = await _authService.GetTokenAsync(cancellationToken);

            IEnumerable<SetDto> sets = await _apiService.GetAsync<IEnumerable<SetDto>>(
                "/api/ExerciseStatistics/sets",
                authResult.Value,
                cancellationToken);

            return sets.Select(SetMapper.ToDomain);
        }

        public async Task<ExerciseSet> AddSetAsync(AddExerciseSetModel addModel, CancellationToken cancellationToken = default)
        {
            OperationResult<string> authResult = await _authService.GetTokenAsync(cancellationToken);

            AddSetDto addSetDto = SetMapper.ToDto(addModel);

            SetDto addedSet = await _apiService.PostAsync<SetDto>(
                "/api/ExerciseStatistics/sets",
                addSetDto,
                authResult.Value,
                cancellationToken);

            return SetMapper.ToDomain(addedSet);
        }

        public async Task<IEnumerable<ExerciseSet>> AddRangeAsync(IEnumerable<AddExerciseSetModel> addModels,
            CancellationToken cancellationToken = default)
        {
            OperationResult<string> authResult = await _authService.GetTokenAsync(cancellationToken);

            IEnumerable<AddSetDto> addSetDtos = addModels.Select(SetMapper.ToDto);

            IEnumerable<SetDto> addedSets = await _apiService.PostAsync<IEnumerable<SetDto>>(
                "/api/ExerciseStatistics/sets/batch",
                addSetDtos,
                authResult.Value,
                cancellationToken);

            return addedSets.Select(SetMapper.ToDomain);
        }
    }
}