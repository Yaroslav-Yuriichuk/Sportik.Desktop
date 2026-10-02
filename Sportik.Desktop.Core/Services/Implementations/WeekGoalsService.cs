using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Goal;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Services.Implementations
{
    internal sealed class WeekGoalsService : IWeekGoalsService
    {
        private readonly IWeekGoalsRepository _remoteWeekGoalsRepository;
        private readonly IWeekGoalsRepository _localWeekGoalsRepository;
        private readonly IRuntimeCacheService _runtimeCacheService;
        private readonly IEventsService _eventsService;

        private IWeekGoalsRepository WeekGoalsRepository
        {
            get
            {
                if (!_runtimeCacheService.TryGet(out AppModeCache appModeCache))
                {
                    return _localWeekGoalsRepository;
                }

                return appModeCache.IsGuest ? _localWeekGoalsRepository : _remoteWeekGoalsRepository;
            }
        }

        public WeekGoalsService(Func<DataSource, IWeekGoalsRepository> weekGoalsRepositoryFactory,
            IRuntimeCacheService runtimeCacheService, IEventsService eventsService)
        {
            _remoteWeekGoalsRepository = weekGoalsRepositoryFactory(DataSource.Remote);
            _localWeekGoalsRepository = weekGoalsRepositoryFactory(DataSource.Local);
            _runtimeCacheService = runtimeCacheService;
            _eventsService = eventsService;
        }

        public async Task<OperationResult<WeekGoal>> GetCurrentWeekGoalAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                WeekGoal weekGoal = await WeekGoalsRepository.GetCurrentWeekGoalAsync(cancellationToken);
                return OperationResult<WeekGoal>.Success(weekGoal);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult<WeekGoal>.Failure(new[] { "Failed to retrieve current week goal." });
            }
        }

        public async Task<OperationResult<ExerciseGoal>> AddGoalAsync(AddExerciseGoalModel addModel, CancellationToken cancellationToken = default)
        {
            try
            {
                ExerciseGoal exerciseGoal = await WeekGoalsRepository.AddGoalAsync(addModel, cancellationToken);
                _eventsService.RaiseEvent(new ExerciseGoalAddedEventArgs(exerciseGoal));

                return OperationResult<ExerciseGoal>.Success(exerciseGoal);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult<ExerciseGoal>.Failure(new[] { "Failed to add exercise goal." });
            }
        }
    }
}