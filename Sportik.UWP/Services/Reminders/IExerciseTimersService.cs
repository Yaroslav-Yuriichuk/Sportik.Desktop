using Sportik.UWP.Core;
using Sportik.UWP.Models;

namespace Sportik.UWP.Services.Reminders
{
    internal interface IExerciseTimersService
    {
        ITimer GetTimer(Exercise exercise);
    }
}
