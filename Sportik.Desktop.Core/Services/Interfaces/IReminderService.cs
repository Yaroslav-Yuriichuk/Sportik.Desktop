using System;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models.Automation;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IReminderService
    {
        event Action<ReminderModeChangedEventArgs> ModeChanged;

        bool IsRunning { get; }

        ReminderMode Mode { get; set; }

        void Start(ReminderMode? mode = null);

        void Stop();

        TState GetExerciseState<TState>(Guid exerciseId) where TState : Enum;

        void AddExercise(Guid exerciseId);

        void RemoveExercise(Guid exerciseId);

        bool IsExerciseAdded(Guid exerciseId);
    }
}
