using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models.Goal;
using Sportik.Desktop.Core.Repositories.Interfaces;

namespace Sportik.Desktop.Infrastructure.Repositories.Implementations
{
    internal sealed class RemoteWeekGoalsRepository : IWeekGoalsRepository
    {
        public async Task<WeekGoal> GetCurrentWeekGoalAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException("Week goals are not supported in the remote repository.");
        }

        public async Task<ExerciseGoal> AddGoalAsync(AddExerciseGoalModel addModel, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException("Adding goals is not supported in the remote repository.");
        }
    }
}