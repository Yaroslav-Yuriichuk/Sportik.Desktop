using System.Collections.Generic;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.Core.Timers;

namespace Sportik.Desktop.Core.Services.Implementations
{
    public sealed class ExerciseTimersService : IExerciseTimersService
    {
        private readonly Dictionary<ReminderMode, Dictionary<int, ITimer>> _timers = new Dictionary<ReminderMode, Dictionary<int, ITimer>>();

        public ITimer GetTimer(Exercise exercise, ReminderMode mode)
        {
            lock (_timers)
            {
                if (!_timers.TryGetValue(mode, out Dictionary<int, ITimer> timers))
                {
                    timers = new Dictionary<int, ITimer>();
                    _timers.Add(mode, timers);
                }

                if (timers.TryGetValue(exercise.Id, out ITimer timer))
                {
                    return timer;
                }

                timer = new DefaultTimerBuilder()
                    .SetInterval(exercise.ExerciseSettings.TimeBetweenSets)
                    .Build();

                timers.Add(exercise.Id, timer);

                return timer;
            }
        }
    }
}
