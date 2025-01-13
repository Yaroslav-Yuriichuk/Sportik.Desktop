using System;
using System.Collections.Generic;
using Sportik.Core;
using Sportik.Helpers;
using Sportik.Models;

namespace Sportik.Services.Notifications
{
    internal sealed class ExerciseTimersService : IExerciseTimersService
    {
        private readonly Dictionary<int, ITimer> _timers = new Dictionary<int, ITimer>();

        public ITimer GetTimer(Exercise exercise)
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
