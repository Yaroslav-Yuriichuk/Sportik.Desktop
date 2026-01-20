using System;

namespace Sportik.Desktop.Core.Events
{
    public sealed class TrainingSetCompleteRequestedEventArgs : EventArgs
    {
        public Guid SetId { get; }

        public TrainingSetCompleteRequestedEventArgs(Guid setId)
        {
            SetId = setId;
        }
    }
}

