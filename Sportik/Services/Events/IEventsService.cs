using System;

namespace Sportik.Services.Events
{
    internal interface IEventsService
    {
        event Action<EventArgs> Event;

        void RaiseEvent(EventArgs args);
    }
}
