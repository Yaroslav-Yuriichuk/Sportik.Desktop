using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Extensions;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;
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

        public async Task<Exercise> AddAsync(AddExerciseModel exercise, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(exercise.Name))
            {
                throw new ArgumentNullException(exercise.Name);
            }

            OperationResult<string> authResult = await _authService.GetTokenAsync(cancellationToken);

            AddExerciseDto addExerciseDto = ExerciseMapper.ToDto(exercise);

            ExerciseDto exerciseDto = await _apiService.PostAsync<ExerciseDto>(
                "/api/Exercises",
                addExerciseDto,
                authResult.Value,
                cancellationToken);

            EnabledExercisesCache enabledExercisesCache = _persistentCacheService.GetOrNew<EnabledExercisesCache>();

            return ExerciseMapper.ToDomain(exerciseDto, enabledExercisesCache.IncludesExercise(exerciseDto.Id));
        }

        public async Task<IEnumerable<Exercise>> AddRangeAsync(IEnumerable<AddExerciseModel> exercises, CancellationToken cancellationToken = default)
        {
            OperationResult<string> authResult = await _authService.GetTokenAsync(cancellationToken);

            IEnumerable<AddExerciseDto> addExerciseDtos = exercises.Select(ExerciseMapper.ToDto);

            IEnumerable<ExerciseDto> exerciseDtos = await _apiService.PostAsync<IEnumerable<ExerciseDto>>(
                "/api/Exercises/batch",
                addExerciseDtos,
                authResult.Value,
                cancellationToken);

            EnabledExercisesCache enabledExercisesCache = _persistentCacheService.GetOrNew<EnabledExercisesCache>();

            return exerciseDtos.Select(e => ExerciseMapper.ToDomain(e, enabledExercisesCache.IncludesExercise(e.Id)));
        }

        public Task<IEnumerable<Exercise>> DeleteAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException("Deleting exercises is not supported in the remote repository.");
        }
    }
}