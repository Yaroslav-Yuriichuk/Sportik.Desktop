using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sportik.Data.Database;
using Sportik.Models.Statistics;

namespace Sportik.Services.Statistics
{
    internal sealed class DayStatisticsDbRepository : IDayStatisticsRepository
    {
        private readonly AppDbContext _dbContext;

        public DayStatisticsDbRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DayStatistics> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.DayStatistics
                .Include(dayStatistics => dayStatistics.ExerciseStatistics)
                    .ThenInclude(exerciseStatistics => exerciseStatistics.Exercise)
                .Include(dayStatistics => dayStatistics.ExerciseStatistics)
                    .ThenInclude(exerciseStatistics => exerciseStatistics.DayStatistics)
                .FirstOrDefaultAsync(dayStatistics => dayStatistics.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<DayStatistics>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.DayStatistics
                .Include(dayStatistics => dayStatistics.ExerciseStatistics)
                    .ThenInclude(exerciseStatistics => exerciseStatistics.Exercise)
                .Include(dayStatistics => dayStatistics.ExerciseStatistics)
                    .ThenInclude(exerciseStatistics => exerciseStatistics.DayStatistics)
                .ToListAsync(cancellationToken);
        }

        public async Task<DayStatistics> AddAsync(DayStatistics entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.DayStatistics.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public async Task<DayStatistics> UpdateAsync(DayStatistics entity, CancellationToken cancellationToken = default)
        {
            _dbContext.DayStatistics.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public async Task<DayStatistics> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            DayStatistics entity = await GetByIdAsync(id, cancellationToken);

            if (entity != null)
            {
                _dbContext.DayStatistics.Remove(entity);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return entity;
        }

        public async Task<DayStatistics> DeleteAsync(DayStatistics entity, CancellationToken cancellationToken = default)
        {
            _dbContext.DayStatistics.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public async Task<DayStatistics> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            return await _dbContext.DayStatistics
                .Include(dayStatistics => dayStatistics.ExerciseStatistics)
                    .ThenInclude(exerciseStatistics => exerciseStatistics.Exercise)
                .Include(dayStatistics => dayStatistics.ExerciseStatistics)
                    .ThenInclude(exerciseStatistics => exerciseStatistics.DayStatistics)
                .FirstOrDefaultAsync(dayStatistics => dayStatistics.Date == date, cancellationToken);
        }
    }
}
