using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Data.Database;

namespace Sportik.Desktop.Data.Repositories.Implementations
{
    public sealed class DayStatisticsDbRepository : IDayStatisticsRepository
    {
        private readonly AppDbContext _dbContext;

        public DayStatisticsDbRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public DayStatistics GetById(int id)
        {
            return _dbContext.DayStatistics
                .Include(dayStatistics => dayStatistics.ExerciseStatistics)
                    .ThenInclude(exerciseStatistics => exerciseStatistics.Exercise)
                .Include(dayStatistics => dayStatistics.ExerciseStatistics)
                    .ThenInclude(exerciseStatistics => exerciseStatistics.DayStatistics)
                .FirstOrDefault(dayStatistics => dayStatistics.Id == id);
        }

        public IEnumerable<DayStatistics> GetAll()
        {
            return _dbContext.DayStatistics
                .Include(dayStatistics => dayStatistics.ExerciseStatistics)
                    .ThenInclude(exerciseStatistics => exerciseStatistics.Exercise)
                .Include(dayStatistics => dayStatistics.ExerciseStatistics)
                    .ThenInclude(exerciseStatistics => exerciseStatistics.DayStatistics)
                .ToList();
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

        public DayStatistics Add(DayStatistics entity)
        {
            _dbContext.DayStatistics.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public async Task<DayStatistics> AddAsync(DayStatistics entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.DayStatistics.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public DayStatistics Update(DayStatistics entity)
        {
            _dbContext.DayStatistics.Update(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public async Task<DayStatistics> UpdateAsync(DayStatistics entity, CancellationToken cancellationToken = default)
        {
            _dbContext.DayStatistics.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public DayStatistics DeleteById(int id)
        {
            DayStatistics entity = GetById(id);

            if (entity != null)
            {
                _dbContext.DayStatistics.Remove(entity);
                _dbContext.SaveChanges();
            }

            return entity;
        }

        public DayStatistics Delete(DayStatistics entity)
        {
            _dbContext.DayStatistics.Remove(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public async Task<DayStatistics> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
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

        public DayStatistics GetByDate(DateTime date)
        {
            return _dbContext.DayStatistics
                .Include(dayStatistics => dayStatistics.ExerciseStatistics)
                    .ThenInclude(exerciseStatistics => exerciseStatistics.Exercise)
                .Include(dayStatistics => dayStatistics.ExerciseStatistics)
                    .ThenInclude(exerciseStatistics => exerciseStatistics.DayStatistics)
                .FirstOrDefault(dayStatistics => dayStatistics.Date == date);
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
