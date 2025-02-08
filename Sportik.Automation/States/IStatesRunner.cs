using System;
using Sportik.Core.Models;

namespace Sportik.Automation.States
{
    internal interface IStatesRunner : IDisposable
    {
        TState GetExerciseState<TState>(Exercise exercise) where TState : Enum;
    }
}
