using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models.Goal;

namespace Sportik.Desktop.Core.Repositories.Interfaces
{
    public interface IWeekGoalsRepository
    {
        Task<WeekGoal> GetCurrentWeekGoalAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<WeekGoal>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<ExerciseGoal> AddGoalAsync(AddExerciseGoalModel addModel, CancellationToken cancellationToken = default);
    }
}