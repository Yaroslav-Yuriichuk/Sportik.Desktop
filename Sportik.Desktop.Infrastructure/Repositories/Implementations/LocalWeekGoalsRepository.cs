using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sportik.Desktop.Core.Extensions;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models.Goal;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Infrastructure.Models;
using Sportik.Desktop.Infrastructure.Persistence;
using Sportik.Desktop.Infrastructure.Persistence.Entities;
using Sportik.Desktop.Infrastructure.Persistence.Mappers;

namespace Sportik.Desktop.Infrastructure.Repositories.Implementations
{
    internal sealed class LocalWeekGoalsRepository : IWeekGoalsRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IPersistentCacheService _persistentCacheService;

        public LocalWeekGoalsRepository(AppDbContext dbContext, IPersistentCacheService persistentCacheService)
        {
            _dbContext = dbContext;
            _persistentCacheService = persistentCacheService;
        }

        public async Task<WeekGoal> GetCurrentWeekGoalAsync(CancellationToken cancellationToken = default)
        {
            DateTime currentDate = DateTimeOffset.Now.Date;

            DateTime firstWeekDayDate = CalendarHelper.GetFirstDayOfWeek(currentDate);
            DateTime lastWeekDayDate = CalendarHelper.GetLastDayOfWeek(currentDate);

            List<UserExerciseGoal> goals = await _dbContext.ExerciseGoals
                .AsNoTracking()
                .Include(g => g.Exercise)
                .ThenInclude(e => e.Settings)
                .Where(g => g.FirstWeekDayDate == firstWeekDayDate)
                .ToListAsync(cancellationToken);


            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.UtcNow);

            DateTimeOffset firstWeekDayDateOffset = new DateTimeOffset(firstWeekDayDate, offset).ToUniversalTime();
            DateTimeOffset lastWeekDayDateOffset = new DateTimeOffset(lastWeekDayDate, offset).ToUniversalTime();

            Dictionary<Guid, int> completedRepetitions = await _dbContext.Sets
                .AsNoTracking()
                .Where(s => s.LoggedAt >= firstWeekDayDateOffset && s.LoggedAt <= lastWeekDayDateOffset)
                .GroupBy(s => s.ExerciseId)
                .Select(g => new
                {
                    ExerciseId = g.Key,
                    CompletedRepetitions = g.Sum(s => s.Repetitions)
                })
                .ToDictionaryAsync(x => x.ExerciseId, x => x.CompletedRepetitions, cancellationToken);

            EnabledExercisesCache enabledExercisesCache = _persistentCacheService.GetOrNew<EnabledExercisesCache>();

            List<ExerciseGoal> exerciseGoals = goals.Select(g => ExerciseGoalMapper.ToDomain(
                g,
                completedRepetitions.GetValueOrDefault(g.ExerciseId, 0),
                enabledExercisesCache.IncludesExercise(g.ExerciseId))).ToList();

            return new WeekGoal(firstWeekDayDate, exerciseGoals);
        }

        public async Task<ExerciseGoal> AddGoalAsync(AddExerciseGoalModel addModel, CancellationToken cancellationToken = default)
        {
            UserExercise exerciseEntity = await _dbContext.Exercises
                .Include(e => e.Settings)
                .FirstOrDefaultAsync(e => e.Id == addModel.ExerciseId, cancellationToken);

            if (exerciseEntity is null)
            {
                return null;
            }

            DateTime firstWeekDayDate = CalendarHelper.GetFirstDayOfWeek(addModel.DayInWeek);
            DateTime lastWeekDayDate = CalendarHelper.GetLastDayOfWeek(addModel.DayInWeek);

            UserExerciseGoal entity = await _dbContext.ExerciseGoals
                .Include(g => g.Exercise)
                .ThenInclude(e => e.Settings)
                .FirstOrDefaultAsync(g => g.ExerciseId == addModel.ExerciseId && g.FirstWeekDayDate == firstWeekDayDate, cancellationToken);

            if (entity != null)
            {
                entity.TargetRepetitions = Math.Max(entity.TargetRepetitions, addModel.Repetitions);
            }
            else
            {
                entity = ExerciseGoalMapper.ToEntity(addModel, exerciseEntity);
                _dbContext.ExerciseGoals.Add(entity);
            }

            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.UtcNow);

            DateTimeOffset firstWeekDayDateOffset = new DateTimeOffset(firstWeekDayDate, offset).ToUniversalTime();
            DateTimeOffset lastWeekDayDateOffset = new DateTimeOffset(lastWeekDayDate, offset).ToUniversalTime();

            int completedRepetitions = await _dbContext.Sets
                .AsNoTracking()
                .Where(s => s.ExerciseId == addModel.ExerciseId && s.LoggedAt >= firstWeekDayDateOffset && s.LoggedAt <= lastWeekDayDateOffset)
                .SumAsync(r => (int?)r.Repetitions, cancellationToken) ?? 0;

            await _dbContext.SaveChangesAsync(cancellationToken);

            EnabledExercisesCache enabledExercisesCache = _persistentCacheService.GetOrNew<EnabledExercisesCache>();

            return ExerciseGoalMapper.ToDomain(entity, completedRepetitions, enabledExercisesCache.IncludesExercise(entity.ExerciseId));
        }
    }
}