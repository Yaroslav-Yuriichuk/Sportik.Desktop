using Sportik.Automation.Models;
using Sportik.Core.Models;
using Sportik.Core.Timers;

namespace Sportik.Automation.Services
{
    public interface IExerciseTimersService
    {
        ITimer GetTimer(Exercise exercise, ReminderMode mode);
    }
}
