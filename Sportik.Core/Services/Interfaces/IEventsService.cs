using System;

namespace Sportik.Core.Services.Interfaces
{
    public interface IEventsService
    {
        event Action<EventArgs> Event;

        void RaiseEvent(EventArgs args);
    }
}
