namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IRuntimeCacheService
    {
        T Get<T>();

        bool TryGet<T>(out T value);

        void Set<T>(T value);

        void Remove<T>();
    }
}
