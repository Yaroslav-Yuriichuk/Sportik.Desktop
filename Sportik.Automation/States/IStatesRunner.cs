using System;
using Sportik.Automation.Models;
using Sportik.Core.Models;

namespace Sportik.Automation.States
{
    internal interface IStatesRunner : IDisposable
    {
        public ReminderMode Mode { get; }

        TState GetExerciseState<TState>(Exercise exercise) where TState : Enum;
    }
}
