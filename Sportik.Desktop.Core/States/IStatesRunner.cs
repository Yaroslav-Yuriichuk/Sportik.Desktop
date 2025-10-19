using System;
using Sportik.Desktop.Core.Models.Automation;

namespace Sportik.Desktop.Core.States
{
    internal interface IStatesRunner : IDisposable
    {
        public ReminderMode Mode { get; }

        TState GetExerciseState<TState>(Guid exerciseId) where TState : Enum;
    }
}
