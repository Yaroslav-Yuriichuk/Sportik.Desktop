using System;
using Sportik.Desktop.Core.States.App;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ApplicationStateChangedEventArgs : EventArgs
    {
        public ApplicationState PreviousState { get; }

        public ApplicationState CurrentState { get; }

        public ApplicationStateChangedEventArgs(ApplicationState previousState, ApplicationState currentState)
        {
            PreviousState = previousState;
            CurrentState = currentState;
        }
    }
}