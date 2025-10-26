using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Backend.Domain.Common;
using Sportik.Desktop.Core.Extensions;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Infrastructure.DTOs.Exercises;
using Sportik.Desktop.Infrastructure.Mappers;
using Sportik.Desktop.Infrastructure.Models;
using Sportik.Desktop.Infrastructure.Services.Interfaces;

namespace Sportik.Desktop.Infrastructure.Repositories.Implementations
{
    internal sealed class RemoteExercisesRepository : IExercisesRepository
    {
        private readonly IApiService _apiService;
        private readonly IAuthService _authService;
        private readonly IPersistentCacheService _persistentCacheService;

        public RemoteExercisesRepository(IApiService apiService, IAuthService authService,
            IPersistentCacheService persistentCacheService)
        {
            _apiService = apiService;
            _authService = authService;
            _persistentCacheService = persistentCacheService;
        }

        public async Task<IEnumerable<Exercise>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            OperationResult<string> authResult = await _authService.GetTokenAsync(cancellationToken);

            IEnumerable<ExerciseDto> exercises = await _apiService.GetAsync<IEnumerable<ExerciseDto>>(
                "/api/Exercises",
                authResult.Value,
                cancellationToken);

            EnabledExercisesCache enabledExercisesCache = _persistentCacheService.GetOrNew<EnabledExercisesCache>();

            return exercises.Select(e => ExerciseMapper.ToDomain(e, enabledExercisesCache.IncludesExercise(e.Id)));
        }

        public async Task<Exercise> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            OperationResult<string> authResult = await _authService.GetTokenAsync(cancellationToken);

            ExerciseDto exercise = await _apiService.GetAsync<ExerciseDto>(
                $"/api/Exercises/{id}",
                authResult.Value,
                cancellationToken);

            EnabledExercisesCache enabledExercisesCache = _persistentCacheService.GetOrNew<EnabledExercisesCache>();

            return ExerciseMapper.ToDomain(exercise, enabledExercisesCache.IncludesExercise(exercise.Id));
        }

        public async Task<IEnumerable<Exercise>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            OperationResult<string> authResult = await _authService.GetTokenAsync(cancellationToken);

            IEnumerable<ExerciseDto> exercises = await _apiService.GetAsync<IEnumerable<ExerciseDto>>(
                "/api/Exercises",
                authResult.Value,
                cancellationToken);

            HashSet<Guid> idsSet = ids as HashSet<Guid> ?? ids.ToHashSet();

            EnabledExercisesCache enabledExercisesCache = _persistentCacheService.GetOrNew<EnabledExercisesCache>();

            return exercises
                .Where(e => idsSet.Contains(e.Id))
                .Select(e => ExerciseMapper.ToDomain(e, enabledExercisesCache.IncludesExercise(e.Id)));
        }
    }
}