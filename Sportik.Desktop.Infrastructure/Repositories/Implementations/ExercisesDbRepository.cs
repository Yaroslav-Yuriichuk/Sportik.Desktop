using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Infrastructure.Database;

namespace Sportik.Desktop.Infrastructure.Repositories.Implementations
{
    public sealed class ExercisesDbRepository : IExercisesRepository
    {
        private readonly AppDbContext _dbContext;

        public ExercisesDbRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Exercise> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Exercises
                .Include(exercise => exercise.ExerciseSettings)
                .FirstOrDefaultAsync(exercise => exercise.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Exercise>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Exercises
                .Include(exercise => exercise.ExerciseSettings)
                .ToListAsync(cancellationToken);
        }

        public async Task<Exercise> AddAsync(Exercise entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.Exercises.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public async Task<Exercise> UpdateAsync(Exercise entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Exercises.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public async Task<Exercise> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Exercise entity = await GetByIdAsync(id, cancellationToken);

            if (entity != null)
            {
                _dbContext.Exercises.Remove(entity);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return entity;
        }

        public async Task<Exercise> DeleteAsync(Exercise entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Exercises.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public async Task<IEnumerable<Exercise>> GetByIdsAsync(IEnumerable<int> exercisesIds, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Exercises
                .Include(exercise => exercise.ExerciseSettings)
                .Where(exercise => exercisesIds.Contains(exercise.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task<Exercise> GetByKindAsync(ExerciseKind exerciseKind, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Exercises
                .Include(exercise => exercise.ExerciseSettings)
                .FirstOrDefaultAsync(exercise => exercise.Kind == exerciseKind, cancellationToken);
        }
    }
}
