using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sportik.Desktop.Core.Extensions;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Infrastructure.Models;
using Sportik.Desktop.Infrastructure.Persistence;
using Sportik.Desktop.Infrastructure.Persistence.Entities;
using Sportik.Desktop.Infrastructure.Persistence.Mappers;

namespace Sportik.Desktop.Infrastructure.Repositories.Implementations
{
    internal sealed class LocalExerciseSettingsRepository : IExerciseSettingsRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IPersistentCacheService _persistentCacheService;

        public LocalExerciseSettingsRepository(AppDbContext dbContext, IPersistentCacheService persistentCacheService)
        {
            _dbContext = dbContext;
            _persistentCacheService = persistentCacheService;
        }

        public async Task<ExerciseSettings> UpdateAsync(ExerciseSettingsDelta delta, Guid exerciseId,
            CancellationToken cancellationToken = default)
        {
            UserExercise exerciseEntity = await _dbContext.Exercises
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == exerciseId, cancellationToken);

            if (exerciseEntity is null)
            {
                return null;
            }

            UserExerciseSettings settingsEntity = await _dbContext.ExerciseSettings
                .FirstOrDefaultAsync(es => es.Id == exerciseEntity.SettingsId, cancellationToken);

            if (settingsEntity is null)
            {
                return null;
            }

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

            if (delta.Change.HasFlag(ExerciseSettingsChange.TargetRepetitions))
            {
                settingsEntity.TargetRepetitions = delta.TargetRepetitions;
            }

            if (delta.Change.HasFlag(ExerciseSettingsChange.TimeBetweenSets))
            {
                settingsEntity.TimeBetweenSets = delta.TimeBetweenSets;
            }

            if (delta.Change.HasFlag(ExerciseSettingsChange.ExecutionTime))
            {
                settingsEntity.ExecutionTime = delta.ExecutionTime;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return ExerciseSettingsMapper.ToDomain(settingsEntity, enabledExercisesCache.IncludesExercise(exerciseId));
        }
    }
}