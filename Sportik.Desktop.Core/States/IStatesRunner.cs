using System;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Automation;

namespace Sportik.Desktop.Core.States
{
    internal interface IStatesRunner : IDisposable
    {
        public ReminderMode Mode { get; }

        TState GetExerciseState<TState>(Exercise exercise) where TState : Enum;
    }
}
