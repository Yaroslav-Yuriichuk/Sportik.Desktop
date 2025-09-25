using System;
using System.Collections.Generic;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Services.Implementations
{
    public sealed class RuntimeCacheService : IRuntimeCacheService
    {
        private readonly Dictionary<Type, object> _cache = new Dictionary<Type, object>();

        public T Get<T>()
        {
            if (_cache.TryGetValue(typeof(T), out object value))
            {
                return (T)value;
            }

            throw new KeyNotFoundException($"Value of type {typeof(T).Name} not found in cache.");
        }

        public bool TryGet<T>(out T value)
        {
            if (_cache.TryGetValue(typeof(T), out object obj))
            {
                value = (T)obj;
                return true;
            }

            value = default;
            return false;
        }

        public void Set<T>(T value)
        {
            _cache[typeof(T)] = value;
        }

        public void Remove<T>()
        {
            _cache.Remove(typeof(T));
        }
    }
}
