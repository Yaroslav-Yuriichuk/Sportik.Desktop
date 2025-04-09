using System.Collections.Generic;
using Sportik.Core.Services.Interfaces;

namespace Sportik.Core.Extensions
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
