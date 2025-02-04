using System;

namespace Sportik.Core.Services.Interfaces
{
    public interface IEventsService
    {
        void AddListener<TEventArgs>(Action<TEventArgs> callback) where TEventArgs : EventArgs;

        void RemoveListener<TEventArgs>(Action<TEventArgs> callback) where TEventArgs : EventArgs;

        void RaiseEvent<TEventArgs>(TEventArgs args) where TEventArgs : EventArgs;
    }
}
