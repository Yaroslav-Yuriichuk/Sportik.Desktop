using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sportik.Data.Database;
using Sportik.Models.Statistics;

namespace Sportik.Services.Statistics
{
    internal sealed class ExerciseStatisticsDbRepository : IExerciseStatisticsRepository
    {
        private readonly AppDbContext _dbContext;

        public ExerciseStatisticsDbRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ExerciseStatistics> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ExerciseStatistics.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IEnumerable<ExerciseStatistics>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ExerciseStatistics.ToListAsync(cancellationToken);
        }

        public async Task<ExerciseStatistics> AddAsync(ExerciseStatistics entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.ExerciseStatistics.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public async Task<ExerciseStatistics> UpdateAsync(ExerciseStatistics entity, CancellationToken cancellationToken = default)
        {
            _dbContext.ExerciseStatistics.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public async Task<ExerciseStatistics> DeleteAsync(int id, CancellationToken cancellationToken = default)
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
