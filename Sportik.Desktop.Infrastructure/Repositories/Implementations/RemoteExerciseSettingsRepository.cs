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

        public async Task<Exercise> UpdateAsync(UpdateExerciseSettingsModel updateModel, CancellationToken cancellationToken = default)
        {
            OperationResult<string> authResult = await _authService.GetTokenAsync(cancellationToken);

            ExerciseSettingsUpdateDto updateDto = ExerciseSettingsUpdateMapper.ToDto(updateModel);

            ExerciseDto exerciseDto = await _apiService.PutAsync<ExerciseDto>(
                "/api/ExerciseSettings",
                updateDto,
                authResult.Value,
                cancellationToken);

            ExerciseSettingsDelta delta = updateModel.Delta;

            EnabledExercisesCache enabledExercisesCache = _persistentCacheService.GetOrNew<EnabledExercisesCache>();

            if (delta.Change.HasFlag(ExerciseSettingsChange.IsEnabled))
            {
                if (delta.IsEnabled)
                {
                    enabledExercisesCache.AddExercise(updateModel.ExerciseId);
                }
                else
                {
                    enabledExercisesCache.RemoveExercise(updateModel.ExerciseId);
                }

                _persistentCacheService.Set(enabledExercisesCache);
            }

            return ExerciseMapper.ToDomain(exerciseDto, enabledExercisesCache.IncludesExercise(exerciseDto.Id));
        }

        public async Task<IEnumerable<Exercise>> UpdateRangeAsync(IEnumerable<UpdateExerciseSettingsModel> updateModels,
            CancellationToken cancellationToken = default)
        {
            updateModels = updateModels as IList<UpdateExerciseSettingsModel> ?? updateModels.ToList();

            OperationResult<string> authResult = await _authService.GetTokenAsync(cancellationToken);

            IEnumerable<ExerciseSettingsUpdateDto> updateDtos = updateModels.Select(ExerciseSettingsUpdateMapper.ToDto);

            IEnumerable<ExerciseDto> exerciseDtos = await _apiService.PutAsync<IEnumerable<ExerciseDto>>(
                "/api/ExerciseSettings/batch",
                updateDtos,
                authResult.Value,
                cancellationToken);

            EnabledExercisesCache enabledExercisesCache = _persistentCacheService.GetOrNew<EnabledExercisesCache>();

            foreach (UpdateExerciseSettingsModel updateModel in updateModels)
            {
                ExerciseSettingsDelta delta = updateModel.Delta;

                if (delta.Change.HasFlag(ExerciseSettingsChange.IsEnabled))
                {
                    if (delta.IsEnabled)
                    {
                        enabledExercisesCache.AddExercise(updateModel.ExerciseId);
                    }
                    else
                    {
                        enabledExercisesCache.RemoveExercise(updateModel.ExerciseId);
                    }
                }
            }

            _persistentCacheService.Set(enabledExercisesCache);

            return exerciseDtos.Select(dto => ExerciseMapper.ToDomain(dto, enabledExercisesCache.IncludesExercise(dto.Id)));
        }
    }
}