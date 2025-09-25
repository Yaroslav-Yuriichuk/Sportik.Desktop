using System;
using System.Collections.Generic;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Services.Implementations
{
    public sealed class EventsService : IEventsService
    {
        private readonly Dictionary<Type, Delegate> _eventListeners = new Dictionary<Type, Delegate>();

        public void AddListener<TEventArgs>(Action<TEventArgs> callback) where TEventArgs : EventArgs
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            Type eventType = typeof(TEventArgs);
            
            if (_eventListeners.TryGetValue(eventType, out Delegate existingDelegate))
            {
                _eventListeners[eventType] = Delegate.Combine(existingDelegate, callback);
            }
            else
            {
                _eventListeners[eventType] = callback;
            }
        }

        public void RemoveListener<TEventArgs>(Action<TEventArgs> callback) where TEventArgs : EventArgs
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            Type eventType = typeof(TEventArgs);
            
            if (_eventListeners.TryGetValue(eventType, out Delegate existingDelegate))
            {
                Delegate newDelegate = Delegate.Remove(existingDelegate, callback);
                
                if (newDelegate == null)
                {
                    _eventListeners.Remove(eventType);
                }
                else
                {
                    _eventListeners[eventType] = newDelegate;
                }
            }
        }

        public void RaiseEvent<TEventArgs>(TEventArgs args) where TEventArgs : EventArgs
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            
            Type eventType = typeof(TEventArgs);
            
            if (_eventListeners.TryGetValue(eventType, out Delegate existingDelegate))
            {
                if (existingDelegate is Action<TEventArgs> action)
                {
                    action.Invoke(args);
                }
            }
        }
    }
}
