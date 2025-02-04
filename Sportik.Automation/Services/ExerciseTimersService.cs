using System.Collections.Generic;
using Sportik.Core.Models;
using Sportik.UWP.Core;
using Sportik.UWP.Helpers;

namespace Sportik.Automation.Services
{
    public sealed class ExerciseTimersService : IExerciseTimersService
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
