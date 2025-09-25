using System;
using System.Collections.Generic;
using Sportik.Desktop.Automation.States;
using Sportik.Desktop.Automation.Models;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Automation.Services
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
