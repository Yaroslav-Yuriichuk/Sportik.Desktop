using System.Collections.Generic;
using Windows.Storage;
using Newtonsoft.Json;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Infrastructure.Services.Implementations
{
    internal sealed class PersistentCacheService : IPersistentCacheService
    {
        public T Get<T>()
        {
            string key = $"{typeof(T).Assembly.FullName}.{typeof(T).FullName}";

            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out object json))
            {
                return JsonConvert.DeserializeObject<T>((string)json);
            }

            throw new KeyNotFoundException($"Value of type {typeof(T).Name} not found in cache.");
        }

        public bool TryGet<T>(out T value)
        {
            string key = $"{typeof(T).Assembly.FullName}.{typeof(T).FullName}";

            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out object json))
            {
                value = JsonConvert.DeserializeObject<T>((string)json);
                return true;
            }

            value = default;
            return false;
        }

        public void Set<T>(T value)
        {
            string key = $"{typeof(T).Assembly.FullName}.{typeof(T).FullName}";
            string json = JsonConvert.SerializeObject(value);

            ApplicationData.Current.LocalSettings.Values[key] = json;
        }

        public void Remove<T>()
        {
            string key = $"{typeof(T).Assembly.FullName}.{typeof(T).FullName}";
            ApplicationData.Current.LocalSettings.Values.Remove(key);
        }
    }
}
