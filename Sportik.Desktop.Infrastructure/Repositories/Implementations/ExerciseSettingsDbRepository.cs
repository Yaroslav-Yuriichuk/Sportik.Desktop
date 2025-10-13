using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Infrastructure.Database;

namespace Sportik.Desktop.Infrastructure.Repositories.Implementations
{
    public sealed class ExerciseSettingsDbRepository : IExerciseSettingsRepository
    {
        private readonly AppDbContext _dbContext;

        public ExerciseSettingsDbRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
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

        public async Task<ExerciseSettings> AddAsync(ExerciseSettings entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.ExerciseSettings.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public async Task<ExerciseSettings> UpdateAsync(ExerciseSettings entity, CancellationToken cancellationToken = default)
        {
            _dbContext.ExerciseSettings.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

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

        public async Task<ExerciseSettings> DeleteAsync(ExerciseSettings entity, CancellationToken cancellationToken = default)
        {
            _dbContext.ExerciseSettings.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public async Task<ExerciseSettings> GetByKindAsync(ExerciseKind exerciseKind, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ExerciseSettings
                .Include(exerciseSettings => exerciseSettings.Exercise)
                .FirstOrDefaultAsync(exerciseSettings => exerciseSettings.Exercise.Kind == exerciseKind, cancellationToken);
        }
    }
}
