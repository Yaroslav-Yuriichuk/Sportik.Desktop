using System;
using Sportik.Desktop.Core.Common.Timers;
using Sportik.Desktop.Core.Models.Automation;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IExerciseTimersService
    {
        ITimer GetTimer(Guid exerciseId, ReminderMode mode, TimeSpan defaultInterval = default);
    }
}
