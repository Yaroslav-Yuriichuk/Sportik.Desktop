using System;

namespace Sportik.Services.Events
{
    internal sealed class EventsService : IEventsService
    {
        public event Action<EventArgs> Event;
        
        public void RaiseEvent(EventArgs args)
        {
            Event?.Invoke(args);
        }
    }
}
