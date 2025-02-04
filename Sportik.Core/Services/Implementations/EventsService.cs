using Sportik.Core.Services.Interfaces;
using System;

namespace Sportik.Core.Services.Implementations
{
    public sealed class EventsService : IEventsService
    {
        public event Action<EventArgs> Event;

        public void RaiseEvent(EventArgs args)
        {
            Event?.Invoke(args);
        }
    }
}
