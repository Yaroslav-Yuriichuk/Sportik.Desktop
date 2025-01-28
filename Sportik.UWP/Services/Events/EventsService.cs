using System;

namespace Sportik.UWP.Services.Events
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
