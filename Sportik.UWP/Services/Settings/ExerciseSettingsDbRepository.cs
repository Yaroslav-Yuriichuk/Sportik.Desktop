using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sportik.UWP.Data.Database;
using Sportik.UWP.Models;
using Sportik.UWP.Models.Settings;

namespace Sportik.UWP.Services.Settings
{
    internal sealed class ExerciseSettingsDbRepository : IExerciseSettingsRepository
    {
        private readonly AppDbContext _dbContext;

        public ExerciseSettingsDbRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ExerciseSettings GetById(int id)
        {
            return _dbContext.ExerciseSettings
                .Include(exerciseSettings => exerciseSettings.Exercise)
                .FirstOrDefault(exerciseSettings => exerciseSettings.Id == id);
        }

        public IEnumerable<ExerciseSettings> GetAll()
        {
            return _dbContext.ExerciseSettings
                .Include(exerciseSettings => exerciseSettings.Exercise)
                .ToList();
        }

        public async Task<ExerciseSettings> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ExerciseSettings
                .Include(exerciseSettings => exerciseSettings.Exercise)
                .FirstOrDefaultAsync(exerciseSettings => exerciseSettings.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<ExerciseSettings>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ExerciseSettings
                .Include(exerciseSettings => exerciseSettings.Exercise)
                .ToListAsync(cancellationToken);
        }

        public ExerciseSettings Add(ExerciseSettings entity)
        {
            _dbContext.ExerciseSettings.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public async Task<ExerciseSettings> AddAsync(ExerciseSettings entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.ExerciseSettings.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            return entity;
        }

        public ExerciseSettings Update(ExerciseSettings entity)
        {
            _dbContext.ExerciseSettings.Update(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public async Task<ExerciseSettings> UpdateAsync(ExerciseSettings entity, CancellationToken cancellationToken = default)
        {
            _dbContext.ExerciseSettings.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            return entity;
        }

        public ExerciseSettings DeleteById(int id)
        {
            ExerciseSettings entity = GetById(id);

            if (entity != null)
            {
                _dbContext.ExerciseSettings.Remove(entity);
                _dbContext.SaveChanges();
            }

            return entity;
        }

        public async Task<ExerciseSettings> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            ExerciseSettings entity = await GetByIdAsync(id, cancellationToken);
            
            if (entity != null)
            {
                _dbContext.ExerciseSettings.Remove(entity);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            
            return entity;
        }

        public ExerciseSettings Delete(ExerciseSettings entity)
        {
            _dbContext.ExerciseSettings.Remove(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public async Task<ExerciseSettings> DeleteAsync(ExerciseSettings entity, CancellationToken cancellationToken = default)
        {
            _dbContext.ExerciseSettings.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            return entity;
        }

        public ExerciseSettings GetByKind(ExerciseKind exerciseKind)
        {
            return _dbContext.ExerciseSettings
                .Include(exerciseSettings => exerciseSettings.Exercise)
                .FirstOrDefault(exerciseSettings => exerciseSettings.Exercise.Kind == exerciseKind);
        }

        public async Task<ExerciseSettings> GetByKindAsync(ExerciseKind exerciseKind, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ExerciseSettings
                .Include(exerciseSettings => exerciseSettings.Exercise)
                .FirstOrDefaultAsync(exerciseSettings => exerciseSettings.Exercise.Kind == exerciseKind, cancellationToken);
        }
    }
}
