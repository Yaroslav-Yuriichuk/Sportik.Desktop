using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Models.Goal;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IWeekGoalsService
    {
        Task<OperationResult<WeekGoal>> GetCurrentWeekGoalAsync(CancellationToken cancellationToken = default);

        Task<OperationResult<ExerciseGoal>> AddGoalAsync(AddExerciseGoalModel addModel, CancellationToken cancellationToken = default);
    }
}