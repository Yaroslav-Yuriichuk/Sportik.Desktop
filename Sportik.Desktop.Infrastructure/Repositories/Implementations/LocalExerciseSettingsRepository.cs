using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sportik.Desktop.Core.Extensions;
using Sportik.Desktop.Core.Models;
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

        public async Task<Exercise> UpdateAsync(UpdateExerciseSettingsModel updateModel, CancellationToken cancellationToken = default)
        {
            UserExercise exerciseEntity = await _dbContext.Exercises
                .Include(e => e.Settings)
                .FirstOrDefaultAsync(e => e.Id == updateModel.ExerciseId, cancellationToken);

            if (exerciseEntity?.Settings is null)
            {
                return null;
            }

            UserExerciseSettings settingsEntity = exerciseEntity.Settings;
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

            ApplyDelta(settingsEntity, delta);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return ExerciseMapper.ToDomain(exerciseEntity, enabledExercisesCache.IncludesExercise(exerciseEntity.Id));
        }

        public async Task<IEnumerable<Exercise>> UpdateRangeAsync(IEnumerable<UpdateExerciseSettingsModel> updateModels,
            CancellationToken cancellationToken = default)
        {
            List<UpdateExerciseSettingsModel> models = updateModels.ToList();

            if (models.Count == 0)
            {
                return Enumerable.Empty<Exercise>();
            }

            List<Guid> exerciseIds = models.Select(x => x.ExerciseId).ToList();

            List<UserExercise> exercises = await _dbContext.Exercises
                .Include(e => e.Settings)
                .Where(e => exerciseIds.Contains(e.Id))
                .ToListAsync(cancellationToken);

            if (exercises.Count == 0)
            {
                return Enumerable.Empty<Exercise>();
            }

            Dictionary<Guid, UserExercise> exercisesById = exercises.ToDictionary(e => e.Id);

            List<Exercise> result = new List<Exercise>();
            EnabledExercisesCache enabledExercisesCache = _persistentCacheService.GetOrNew<EnabledExercisesCache>();

            foreach (UpdateExerciseSettingsModel updateModel in models)
            {
                if (!exercisesById.TryGetValue(updateModel.ExerciseId, out UserExercise exerciseEntity))
                {
                    continue;
                }

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

                ApplyDelta(exerciseEntity.Settings, delta);

                result.Add(ExerciseMapper.ToDomain(exerciseEntity, enabledExercisesCache.IncludesExercise(exerciseEntity.Id)));
            }

            _persistentCacheService.Set(enabledExercisesCache);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return result;
        }

        private static void ApplyDelta(UserExerciseSettings settings, ExerciseSettingsDelta delta)
        {
            if (delta.Change.HasFlag(ExerciseSettingsChange.TargetRepetitions))
            {
                settings.TargetRepetitions = delta.TargetRepetitions;
            }

            if (delta.Change.HasFlag(ExerciseSettingsChange.TimeBetweenSets))
            {
                settings.TimeBetweenSets = delta.TimeBetweenSets;
            }

            if (delta.Change.HasFlag(ExerciseSettingsChange.ExecutionTime))
            {
                settings.ExecutionTime = delta.ExecutionTime;
            }
        }
    }
}