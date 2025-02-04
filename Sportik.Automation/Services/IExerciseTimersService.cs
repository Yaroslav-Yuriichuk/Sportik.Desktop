using Sportik.Core.Models;
using Sportik.UWP.Core;

namespace Sportik.Automation.Services
{
    public interface IExerciseTimersService
    {
        ITimer GetTimer(Exercise exercise);
    }
}
