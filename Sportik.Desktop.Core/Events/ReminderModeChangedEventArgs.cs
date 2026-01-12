using System;
using Sportik.Desktop.Core.Models.Automation;

namespace Sportik.Desktop.Core.Events
{
    public sealed class ReminderModeChangedEventArgs : EventArgs
    {
        public ReminderMode CurrentMode { get; }

        public ReminderModeChangedEventArgs(ReminderMode currentMode)
        {
            CurrentMode = currentMode;
        }
    }
}