using Sportik.Core;
using Sportik.Helpers;
using Sportik.Models;

namespace Sportik.Services.Notifications
{
    internal interface IExerciseTimersService
    {
        ITimer GetTimer(Exercise exercise);
    }
}
