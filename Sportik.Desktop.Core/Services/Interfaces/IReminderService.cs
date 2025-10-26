using System;
using Sportik.Desktop.Core.Models.Automation;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IReminderService
    {
        bool IsRunning { get; }

        ReminderMode Mode { get; set; }

        void Start(ReminderMode mode = default);

        void Stop();

        TState GetExerciseState<TState>(Guid exerciseId) where TState : Enum;

        void AddExercise(Guid exerciseId);

        void RemoveExercise(Guid exerciseId);

        bool IsExerciseAdded(Guid exerciseId);
    }
}
