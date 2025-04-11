using System;
using System.Collections.Generic;
using Sportik.Automation.Models;
using Sportik.Automation.States;
using Sportik.Core.Models;

namespace Sportik.Automation.Services
{
    public interface IReminderService
    {
        bool IsRunning { get; }

        ReminderMode Mode { get; set; }

        void Start(IEnumerable<Exercise> exercises, ReminderMode mode = default);

        void Stop();

        TState GetExerciseState<TState>(Exercise exercise) where TState : Enum;
    }
}
