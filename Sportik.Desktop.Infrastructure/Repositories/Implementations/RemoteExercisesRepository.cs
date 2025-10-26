using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Backend.Domain.Common;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Infrastructure.DTOs.Exercises;
using Sportik.Desktop.Infrastructure.Mappers;
using Sportik.Desktop.Infrastructure.Services.Interfaces;

namespace Sportik.Desktop.Infrastructure.Repositories.Implementations
{
    internal sealed class RemoteExercisesRepository : IExercisesRepository
    {
        private readonly IApiService _apiService;
        private readonly IAuthService _authService;

        public RemoteExercisesRepository(IApiService apiService, IAuthService authService)
        {
            _apiService = apiService;
            _authService = authService;
        }

        public async Task<IEnumerable<Exercise>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            OperationResult<string> authResult = await _authService.LoginAsync("hello@gmail.com", "Hohoho123", cancellationToken);

            IEnumerable<ExerciseDto> exercises = await _apiService.GetAsync<IEnumerable<ExerciseDto>>(
                "/api/Exercises",
                authResult.Value,
                cancellationToken);

            return exercises.Select(e => ExerciseMapper.ToDomain(e, true));
        }

        public async Task<Exercise> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            ExerciseDto exercise = await _apiService.GetAsync<ExerciseDto>(
                $"/api/Exercises/{id}",
                "",
                cancellationToken);

            return ExerciseMapper.ToDomain(exercise, true);
        }

        public async Task<IEnumerable<Exercise>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            IEnumerable<ExerciseDto> exercises = await _apiService.GetAsync<IEnumerable<ExerciseDto>>(
                "/api/Exercises",
                "",
                cancellationToken);

            HashSet<Guid> idsSet = ids as HashSet<Guid> ?? ids.ToHashSet();

            return exercises
                .Where(e => idsSet.Contains(e.Id))
                .Select(e => ExerciseMapper.ToDomain(e, true));
        }
    }
}