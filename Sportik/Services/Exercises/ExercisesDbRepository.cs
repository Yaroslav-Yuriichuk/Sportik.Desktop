using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sportik.Data.Database;
using Sportik.Models;

namespace Sportik.Services.Exercises
{
    internal sealed class ExercisesDbRepository : IExercisesRepository
    {
        private readonly AppDbContext _dbContext;

        public ExercisesDbRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Exercise GetById(int id)
        {
            return _dbContext.Exercises
                .Include(exercise => exercise.ExerciseSettings)
                .FirstOrDefault(exercise => exercise.Id == id);
        }

        public IEnumerable<Exercise> GetAll()
        {
            return _dbContext.Exercises
                .Include(exercise => exercise.ExerciseSettings)
                .ToList();
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

        public Exercise Add(Exercise entity)
        {
            _dbContext.Exercises.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public async Task<Exercise> AddAsync(Exercise entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.Exercises.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public Exercise Update(Exercise entity)
        {
            _dbContext.Exercises.Update(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public async Task<Exercise> UpdateAsync(Exercise entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Exercises.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public Exercise DeleteById(int id)
        {
            Exercise entity = GetById(id);

            if (entity != null)
            {
                _dbContext.Exercises.Remove(entity);
                _dbContext.SaveChanges();
            }

            return entity;
        }

        public Exercise Delete(Exercise entity)
        {
            _dbContext.Exercises.Remove(entity);
            _dbContext.SaveChanges();

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

        public Exercise GetByKind(ExerciseKind exerciseKind)
        {
            return _dbContext.Exercises
                .Include(exercise => exercise.ExerciseSettings)
                .FirstOrDefault(exercise => exercise.Kind == exerciseKind);
        }

        public async Task<Exercise> GetByKindAsync(ExerciseKind exerciseKind, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Exercises
                .Include(exercise => exercise.ExerciseSettings)
                .FirstOrDefaultAsync(exercise => exercise.Kind == exerciseKind, cancellationToken);
        }
    }
}
