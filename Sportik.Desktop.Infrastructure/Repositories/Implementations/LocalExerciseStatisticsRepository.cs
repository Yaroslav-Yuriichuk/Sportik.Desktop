using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Extensions;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Infrastructure.Models;
using Sportik.Desktop.Infrastructure.Persistence;
using Sportik.Desktop.Infrastructure.Persistence.Entities;
using Sportik.Desktop.Infrastructure.Persistence.Mappers;

namespace Sportik.Desktop.Infrastructure.Repositories.Implementations
{
    internal sealed class LocalExerciseStatisticsRepository : IExerciseStatisticsRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IPersistentCacheService _persistentCacheService;

        public LocalExerciseStatisticsRepository(AppDbContext dbContext, IPersistentCacheService persistentCacheService)
        {
            _dbContext = dbContext;
            _persistentCacheService = persistentCacheService;
        }

        public async Task<IEnumerable<WeekStatistics>> GetWeeklyAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<UserSet> sets = await _dbContext.Sets
                .AsNoTracking()
                .Include(s => s.Exercise)
                .ThenInclude(e => e.Settings)
                .ToListAsync(cancellationToken);

            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.UtcNow);

            EnabledExercisesCache enabledExercisesCache = _persistentCacheService.GetOrNew<EnabledExercisesCache>();

            IEnumerable<IGrouping<DateTime, ExerciseStatistics>> groupedStatistics = sets
                .OrderBy(s => s.LoggedAt)
                .GroupBy(s => s.LoggedAt.ToOffset(offset).Date)
                .Select(group =>
                {
                    DateTime groupKey = group.Key;

                    IEnumerable<ExerciseStatistics> exerciseStatistics = group
                        .GroupBy(s => s.ExerciseId)
                        .Select(g =>
                        {
                            UserExercise exercise = g.First().Exercise;

                            return new ExerciseStatistics(
                                ExerciseMapper.ToDomain(exercise, enabledExercisesCache.IncludesExercise(exercise.Id)),
                                g
                                    .Select(s => SetMapper.ToDomain(s, offset))
                                    .ToList());
                        });

                    return (IGrouping<DateTime, ExerciseStatistics>)new Grouping<DateTime, ExerciseStatistics>(groupKey, exerciseStatistics);
                });

            IEnumerable<DayStatistics> dayStatistics = groupedStatistics
                .Select(group => new DayStatistics(
                    group.Key,
                    group.ToList()))
                .OrderByDescending(statistics => statistics.Date);

            IEnumerable<WeekStatistics> weekStatistics = dayStatistics
                .GroupBy(statistics => CalendarHelper.GetFirstDayOfWeek(statistics.Date))
                .Select(group =>
                {
                    DateTime firstWeekDayDate = group.Key;
                    DateTime lastWeekDayDate = CalendarHelper.GetLastDayOfWeek(firstWeekDayDate);

                    return new WeekStatistics(firstWeekDayDate, lastWeekDayDate, group.ToList());
                });

            return weekStatistics.OrderByDescending(StatisticsHelper.GetFirstDayDate);
        }

        public async Task<IEnumerable<AggregatedRepetitionsExerciseStatistics>> GetAggregatedRepetitionsAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<UserSet> sets = await _dbContext.Sets
                .AsNoTracking()
                .Include(s => s.Exercise)
                .ThenInclude(e => e.Settings)
                .ToListAsync(cancellationToken);

            EnabledExercisesCache enabledExercisesCache = _persistentCacheService.GetOrNew<EnabledExercisesCache>();

            IEnumerable<AggregatedRepetitionsExerciseStatistics> aggregatedStatistics = sets
                .GroupBy(s => s.ExerciseId)
                .Select(group =>
                {
                    UserExercise exercise = group.First().Exercise;

                    return new AggregatedRepetitionsExerciseStatistics(
                        ExerciseMapper.ToDomain(exercise, enabledExercisesCache.IncludesExercise(exercise.Id)),
                        group.Sum(s => s.Repetitions));
                });

            return aggregatedStatistics;
        }

        public async Task<IEnumerable<AggregatedSetsExerciseStatistics>> GetAggregatedSetsAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<UserSet> sets = await _dbContext.Sets
                .AsNoTracking()
                .Include(s => s.Exercise)
                .ThenInclude(e => e.Settings)
                .ToListAsync(cancellationToken);

            EnabledExercisesCache enabledExercisesCache = _persistentCacheService.GetOrNew<EnabledExercisesCache>();

            IEnumerable<AggregatedSetsExerciseStatistics> aggregatedStatistics = sets
                .GroupBy(s => s.ExerciseId)
                .Select(group =>
                {
                    UserExercise exercise = group.First().Exercise;

                    return new AggregatedSetsExerciseStatistics(
                        ExerciseMapper.ToDomain(exercise, enabledExercisesCache.IncludesExercise(exercise.Id)),
                        group.Count());
                });

            return aggregatedStatistics;
        }

        public async Task<IEnumerable<ExerciseSet>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            List<UserSet> setEntities = await _dbContext.Sets
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.UtcNow);

            return setEntities.Select(s => SetMapper.ToDomain(s, offset));
        }

        public async Task<ExerciseSet> AddSetAsync(AddExerciseSetModel addModel, CancellationToken cancellationToken = default)
        {
            UserExercise exerciseEntity = await _dbContext.Exercises
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == addModel.ExerciseId, cancellationToken);

            if (exerciseEntity is null)
            {
                return null;
            }

            UserSet setEntity = SetMapper.ToEntity(addModel);

            _dbContext.Sets.Add(setEntity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.UtcNow);

            return SetMapper.ToDomain(setEntity, offset);
        }

        public async Task<IEnumerable<ExerciseSet>> AddRangeAsync(IEnumerable<AddExerciseSetModel> addModels,
            CancellationToken cancellationToken = default)
        {
            addModels = addModels as IList<AddExerciseSetModel> ?? addModels.ToList();
            List<Guid> exerciseIds = addModels.Select(m => m.ExerciseId).Distinct().ToList();

            HashSet<Guid> existingExerciseIds = (await _dbContext.Exercises
                    .AsNoTracking()
                    .Where(e => exerciseIds.Contains(e.Id))
                    .Select(e => e.Id)
                    .ToListAsync(cancellationToken))
                .ToHashSet();

            List<UserSet> setEntities = new List<UserSet>();

            foreach (AddExerciseSetModel addModel in addModels)
            {
                if (!existingExerciseIds.Contains(addModel.ExerciseId))
                {
                    continue;
                }

                UserSet setEntity = SetMapper.ToEntity(addModel);
                setEntities.Add(setEntity);
            }

            _dbContext.Sets.AddRange(setEntities);
            await _dbContext.SaveChangesAsync(cancellationToken);

            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.UtcNow);

            return setEntities.Select(s => SetMapper.ToDomain(s, offset));
        }
    }
}