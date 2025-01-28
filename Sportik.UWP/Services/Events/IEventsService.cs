using System;

namespace Sportik.UWP.Services.Events
{
    internal interface IEventsService
    {
        event Action<EventArgs> Event;

        void RaiseEvent(EventArgs args);
    }
}
