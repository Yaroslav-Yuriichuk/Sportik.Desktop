using System.Collections.Generic;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Extensions
{
    public static class CachingExtension
    {
        public static T GetOrNew<T>(this IRuntimeCacheService runtimeCacheService) where T : new()
        {
            try
            {
                return runtimeCacheService.Get<T>();
            }
            catch (KeyNotFoundException)
            {
                return new T();
            }
        }

        public static T GetOrNew<T>(this IPersistentCacheService persistentCacheService) where T : new()
        {
            try
            {
                return persistentCacheService.Get<T>();
            }
            catch (KeyNotFoundException)
            {
                return new T();
            }
        }
    }
}
