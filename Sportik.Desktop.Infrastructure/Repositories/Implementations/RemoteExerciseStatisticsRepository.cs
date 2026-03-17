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

        public async Task<ExerciseSet> AddSetAsync(ExerciseSet set, Guid exerciseId, CancellationToken cancellationToken = default)
        {
            OperationResult<string> authResult = await _authService.GetTokenAsync(cancellationToken);

            AddSetDto addSetDto = SetMapper.ToDto(set, exerciseId);

            SetDto addedSet = await _apiService.PostAsync<SetDto>(
                "/api/ExerciseStatistics/sets",
                addSetDto,
                authResult.Value,
                cancellationToken);

            return SetMapper.ToDomain(addedSet);
        }
    }
}