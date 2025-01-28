using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sportik.UWP.Services
{
    internal interface IAsyncRepository<T>
    {
        Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        
        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
        
        Task<T> DeleteByIdAsync(int id, CancellationToken cancellationToken = default);
        
        Task<T> DeleteAsync(T entity, CancellationToken cancellationToken = default);
    }
}
