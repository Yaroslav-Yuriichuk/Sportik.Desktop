namespace Sportik.Core.Services.Interfaces
{
    public interface IPersistentCacheService
    {
        T Get<T>();

        bool TryGet<T>(out T value);

        void Set<T>(T value);

        void Remove<T>();
    }
}
