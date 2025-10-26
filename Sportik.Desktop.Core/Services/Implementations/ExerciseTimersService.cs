using System;
using System.Collections.Generic;
using Sportik.Desktop.Core.Common.Timers;
using Sportik.Desktop.Core.Models.Automation;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Services.Implementations
{
    internal sealed class ExerciseTimersService : IExerciseTimersService
    {
        private readonly Dictionary<ReminderMode, Dictionary<Guid, ITimer>> _timers = new Dictionary<ReminderMode, Dictionary<Guid, ITimer>>();

        public ITimer GetTimer(Guid exerciseId, ReminderMode mode, TimeSpan defaultInterval = default)
        {
            lock (_timers)
            {
                if (!_timers.TryGetValue(mode, out Dictionary<Guid, ITimer> timers))
                {
                    timers = new Dictionary<Guid, ITimer>();
                    _timers.Add(mode, timers);
                }

                if (timers.TryGetValue(exerciseId, out ITimer timer))
                {
                    return timer;
                }

                timer = new DefaultTimerBuilder()
                    .SetInterval(defaultInterval <= TimeSpan.Zero ? TimeSpan.FromMinutes(1) : defaultInterval)
                    .Build();

                timers.Add(exerciseId, timer);

                return timer;
            }
        }
    }
}
