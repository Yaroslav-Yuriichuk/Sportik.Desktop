using System;
using Sportik.Desktop.Automation.Models;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Automation.States
{
    internal interface IStatesRunner : IDisposable
    {
        public ReminderMode Mode { get; }

        TState GetExerciseState<TState>(Exercise exercise) where TState : Enum;
    }
}
