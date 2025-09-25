using Sportik.Desktop.Automation.Models;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Timers;

namespace Sportik.Desktop.Automation.Services
{
    public interface IExerciseTimersService
    {
        ITimer GetTimer(Exercise exercise, ReminderMode mode);
    }
}
