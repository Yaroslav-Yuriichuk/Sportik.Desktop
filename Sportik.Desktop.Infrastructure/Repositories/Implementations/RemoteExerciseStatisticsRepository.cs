using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Backend.Domain.Common;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Infrastructure.DTOs.Statistics;
using Sportik.Desktop.Infrastructure.Mappers.Statistics;
using Sportik.Desktop.Infrastructure.Services.Interfaces;

namespace Sportik.Desktop.Infrastructure.Repositories.Implementations
{
    internal sealed class RemoteExerciseStatisticsRepository : IExerciseStatisticsRepository
    {
        private readonly IApiService _apiService;
        private readonly IAuthService _authService;

        public RemoteExerciseStatisticsRepository(IApiService apiService, IAuthService authService)
        {
            _apiService = apiService;
            _authService = authService;
        }

        public async Task<IEnumerable<WeekStatistics>> GetWeeklyAsync(CancellationToken cancellationToken = default)
        {
            OperationResult<string> authResult = await _authService.GetTokenAsync(cancellationToken);

            IEnumerable<WeekStatisticsDto> weekStatistics = await _apiService.GetAsync<IEnumerable<WeekStatisticsDto>>(
                "/api/ExerciseStatistics/weekly",
                authResult.Value,
                cancellationToken);

            return weekStatistics.Select(WeekStatisticsMapper.ToDomain);
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