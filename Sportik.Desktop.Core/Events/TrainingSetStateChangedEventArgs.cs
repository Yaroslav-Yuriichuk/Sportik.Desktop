using System;
using Sportik.Desktop.Core.States.Training;

namespace Sportik.Desktop.Core.Events
{
    public sealed class TrainingSetStateChangedEventArgs : EventArgs
    {
        public Guid SetId { get; }

        public TrainingSetState PreviousState { get; }

        public TrainingSetState CurrentState { get; }

        public TrainingSetStateChangedEventArgs(Guid setId, TrainingSetState previousState, TrainingSetState currentState)
        {
            SetId = setId;
            PreviousState = previousState;
            CurrentState = currentState;
        }
    }
}

