using System;
using System.Collections.Generic;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Automation;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IReminderService
    {
        bool IsRunning { get; }

        ReminderMode Mode { get; set; }

        void Start(IEnumerable<Guid> exercises, ReminderMode mode = default);

        void Stop();

        TState GetExerciseState<TState>(Guid exerciseId) where TState : Enum;
    }
}
