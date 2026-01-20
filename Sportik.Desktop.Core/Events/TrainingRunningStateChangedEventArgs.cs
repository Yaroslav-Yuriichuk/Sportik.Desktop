using System;
using Sportik.Desktop.Core.Models.Training;

namespace Sportik.Desktop.Core.Events
{
    public sealed class TrainingRunningStateChangedEventArgs : EventArgs
    {
        public bool IsRunning { get; }

        public TrainingStopReason StopReason { get; }

        public TrainingRunningStateChangedEventArgs(bool isRunning, TrainingStopReason stopReason = TrainingStopReason.None)
        {
            IsRunning = isRunning;
            StopReason = stopReason;
        }
    }
}