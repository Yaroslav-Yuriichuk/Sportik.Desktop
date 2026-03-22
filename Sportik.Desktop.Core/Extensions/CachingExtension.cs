using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Extensions
{
    public static class CachingExtension
    {
        public static T GetOrNew<T>(this IRuntimeCacheService runtimeCacheService) where T : new()
        {
            return runtimeCacheService.TryGet(out T result) ? result : new T();
        }

        public static T GetOrNew<T>(this IPersistentCacheService persistentCacheService) where T : new()
        {
            return persistentCacheService.TryGet(out T result) ? result : new T();
        }
    }
}
