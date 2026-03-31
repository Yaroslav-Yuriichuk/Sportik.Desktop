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
    internal sealed class LocalExercisesRepository : IExercisesRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IPersistentCacheService _persistentCacheService;

        public LocalExercisesRepository(AppDbContext dbContext, IPersistentCacheService persistentCacheService)
        {
            _dbContext = dbContext;
            _persistentCacheService = persistentCacheService;
        }

        public async Task<IEnumerable<Exercise>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            List<UserExercise> entities = await _dbContext.Exercises
                .AsNoTracking()
                .Include(e => e.Settings)
                .ToListAsync(cancellationToken);

            EnabledExercisesCache enabledExercisesCache = _persistentCacheService.GetOrNew<EnabledExercisesCache>();

            return entities.Select(e => ExerciseMapper.ToDomain(e, enabledExercisesCache.IncludesExercise(e.Id)));
        }

        public async Task<Exercise> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            UserExercise entity = await _dbContext.Exercises
                .AsNoTracking()
                .Include(e => e.Settings)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (entity is null)
            {
                return null;
            }

            EnabledExercisesCache enabledExercisesCache = _persistentCacheService.GetOrNew<EnabledExercisesCache>();

            return ExerciseMapper.ToDomain(entity, enabledExercisesCache.IncludesExercise(entity.Id));
        }

        public async Task<IEnumerable<Exercise>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            List<UserExercise> entities = await _dbContext.Exercises
                .AsNoTracking()
                .Include(e => e.Settings)
                .Where(e => ids.Contains(e.Id))
                .ToListAsync(cancellationToken);

            EnabledExercisesCache enabledExercisesCache = _persistentCacheService.GetOrNew<EnabledExercisesCache>();

            return entities.Select(e => ExerciseMapper.ToDomain(e, enabledExercisesCache.IncludesExercise(e.Id)));
        }

        public async Task<Exercise> AddAsync(AddExerciseModel exercise, CancellationToken cancellationToken = default)
        {
            UserExercise entity = ExerciseMapper.ToEntity(exercise);

            await _dbContext.Exercises.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            EnabledExercisesCache enabledExercisesCache = _persistentCacheService.GetOrNew<EnabledExercisesCache>();

            if (exercise.Settings.IsEnabled)
            {
                enabledExercisesCache.AddExercise(entity.Id);
                _persistentCacheService.Set(enabledExercisesCache);
            }

            return ExerciseMapper.ToDomain(entity, enabledExercisesCache.IncludesExercise(entity.Id));
        }

        public async Task<IEnumerable<Exercise>> AddRangeAsync(IEnumerable<AddExerciseModel> exercises, CancellationToken cancellationToken = default)
        {
            IList<AddExerciseModel> exerciseModels = exercises as IList<AddExerciseModel> ?? exercises.ToList();
            IList<UserExercise> entities = exerciseModels.Select(ExerciseMapper.ToEntity).ToList();

            await _dbContext.Exercises.AddRangeAsync(entities, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            EnabledExercisesCache enabledExercisesCache = _persistentCacheService.GetOrNew<EnabledExercisesCache>();

            for (int i = 0; i < exerciseModels.Count; i++)
            {
                AddExerciseModel exercise = exerciseModels[i];
                UserExercise entity = entities[i];

                if (exercise.Settings.IsEnabled)
                {
                    enabledExercisesCache.AddExercise(entity.Id);
                }
            }

            _persistentCacheService.Set(enabledExercisesCache);

            return entities.Select(e => ExerciseMapper.ToDomain(e, enabledExercisesCache.IncludesExercise(e.Id)));
        }

        public async Task<IEnumerable<Exercise>> DeleteAllAsync(CancellationToken cancellationToken = default)
        {
            List<UserExercise> entities = await _dbContext.Exercises
                .Include(e => e.Settings)
                .ToListAsync(cancellationToken);

            _dbContext.Exercises.RemoveRange(entities);
            await _dbContext.SaveChangesAsync(cancellationToken);

            EnabledExercisesCache enabledExercisesCache = _persistentCacheService.GetOrNew<EnabledExercisesCache>();

            foreach (UserExercise entity in entities)
            {
                enabledExercisesCache.RemoveExercise(entity.Id);
            }

            _persistentCacheService.Set(enabledExercisesCache);

            return entities.Select(e => ExerciseMapper.ToDomain(e, enabledExercisesCache.IncludesExercise(e.Id)));
        }
    }
}