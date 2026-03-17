using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Extensions;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Infrastructure.DTOs.Exercises;
using Sportik.Desktop.Infrastructure.DTOs.Settings;
using Sportik.Desktop.Infrastructure.Mappers;
using Sportik.Desktop.Infrastructure.Models;
using Sportik.Desktop.Infrastructure.Services.Interfaces;

namespace Sportik.Desktop.Infrastructure.Repositories.Implementations
{
    internal sealed class RemoteExerciseSettingsRepository : IExerciseSettingsRepository
    {
        private readonly IApiService _apiService;
        private readonly IAuthService _authService;
        private readonly IPersistentCacheService _persistentCacheService;

        public RemoteExerciseSettingsRepository(IApiService apiService, IAuthService authService,
            IPersistentCacheService persistentCacheService)
        {
            _apiService = apiService;
            _authService = authService;
            _persistentCacheService = persistentCacheService;
        }

        public async Task<ExerciseSettings> UpdateAsync(ExerciseSettingsDelta delta, Guid exerciseId,
            CancellationToken cancellationToken = default)
        {
            OperationResult<string> authResult = await _authService.GetTokenAsync(cancellationToken);

            ExerciseSettingsUpdateDto updateDto = ExerciseSettingsUpdateMapper.ToDto(delta, exerciseId);

            ExerciseSettingsDto exerciseSettingsDto = await _apiService.PutAsync<ExerciseSettingsDto>(
                "/api/ExerciseSettings",
                updateDto,
                authResult.Value,
                cancellationToken);

            EnabledExercisesCache enabledExercisesCache = _persistentCacheService.GetOrNew<EnabledExercisesCache>();

            if (delta.Change.HasFlag(ExerciseSettingsChange.IsEnabled))
            {
                if (delta.IsEnabled)
                {
                    enabledExercisesCache.AddExercise(exerciseId);
                }
                else
                {
                    enabledExercisesCache.RemoveExercise(exerciseId);
                }

                _persistentCacheService.Set(enabledExercisesCache);
            }

            return ExerciseSettingsMapper.ToDomain(exerciseSettingsDto, enabledExercisesCache.IncludesExercise(exerciseId));
        }
    }
}