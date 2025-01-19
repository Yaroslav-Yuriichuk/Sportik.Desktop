using Sportik.Core;
using Sportik.Models;

namespace Sportik.Services.Reminders
{
    internal interface IExerciseTimersService
    {
        ITimer GetTimer(Exercise exercise);
    }
}
