using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Timers;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IExerciseTimersService
    {
        ITimer GetTimer(Exercise exercise, ReminderMode mode);
    }
}
