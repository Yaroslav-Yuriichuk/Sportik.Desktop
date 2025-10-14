using Sportik.Desktop.Core.Common.Timers;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Automation;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IExerciseTimersService
    {
        ITimer GetTimer(Exercise exercise, ReminderMode mode);
    }
}
