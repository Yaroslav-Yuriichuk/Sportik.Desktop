using System.Collections.Generic;
using Sportik.UWP.Core;
using Sportik.UWP.Helpers;
using Sportik.UWP.Models;

namespace Sportik.UWP.Services.Reminders
{
    internal sealed class ExerciseTimersService : IExerciseTimersService
    {
        private readonly Dictionary<int, ITimer> _timers = new Dictionary<int, ITimer>();

        public ITimer GetTimer(Exercise exercise)
        {
            lock (_timers)
            {
                if (_timers.TryGetValue(exercise.Id, out ITimer timer))
                {
                    return timer;
                }

                timer = new DefaultTimerBuilder()
                    .SetInterval(exercise.ExerciseSettings.TimeBetweenSets)
                    .Build();

                _timers.Add(exercise.Id, timer);

                return timer;
            }
        }
    }
}
