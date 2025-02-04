using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sportik.Core.Models.Statistics;
using Sportik.Core.Repositories.Interfaces;
using Sportik.Data.Database;

namespace Sportik.Data.Repositories.Implementations
{
    public sealed class ExerciseStatisticsDbRepository : IExerciseStatisticsRepository
    {
        private readonly AppDbContext _dbContext;

        public ExerciseStatisticsDbRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ExerciseStatistics GetById(int id)
        {
            return _dbContext.ExerciseStatistics
                .Include(exerciseStatistics => exerciseStatistics.Exercise)
                .FirstOrDefault(exerciseStatistics => exerciseStatistics.Id == id);
        }

        public IEnumerable<ExerciseStatistics> GetAll()
        {
            return _dbContext.ExerciseStatistics
                .Include(exerciseStatistics => exerciseStatistics.Exercise)
                .ToList();
        }

        public async Task<ExerciseStatistics> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ExerciseStatistics
                .Include(exerciseStatistics => exerciseStatistics.Exercise)
                .FirstOrDefaultAsync(exerciseStatistics => exerciseStatistics.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<ExerciseStatistics>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ExerciseStatistics
                .Include(exerciseStatistics => exerciseStatistics.Exercise)
                .ToListAsync(cancellationToken);
        }

        public ExerciseStatistics Add(ExerciseStatistics entity)
        {
            _dbContext.ExerciseStatistics.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public async Task<ExerciseStatistics> AddAsync(ExerciseStatistics entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.ExerciseStatistics.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public ExerciseStatistics Update(ExerciseStatistics entity)
        {
            _dbContext.ExerciseStatistics.Update(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public async Task<ExerciseStatistics> UpdateAsync(ExerciseStatistics entity, CancellationToken cancellationToken = default)
        {
            _dbContext.ExerciseStatistics.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public ExerciseStatistics DeleteById(int id)
        {
            ExerciseStatistics entity = GetById(id);

            if (entity != null)
            {
                _dbContext.ExerciseStatistics.Remove(entity);
                _dbContext.SaveChanges();
            }

            return entity;
        }

        public ExerciseStatistics Delete(ExerciseStatistics entity)
        {
            _dbContext.ExerciseStatistics.Remove(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public async Task<ExerciseStatistics> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            ExerciseStatistics entity = await GetByIdAsync(id, cancellationToken);

            if (entity != null)
            {
                _dbContext.ExerciseStatistics.Remove(entity);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return entity;
        }

        public async Task<ExerciseStatistics> DeleteAsync(ExerciseStatistics entity, CancellationToken cancellationToken = default)
        {
            _dbContext.ExerciseStatistics.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }
    }
}
