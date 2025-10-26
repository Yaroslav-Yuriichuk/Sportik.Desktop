using System.Threading;
using System.Threading.Tasks;

namespace Sportik.Desktop.Infrastructure.Services.Interfaces
{
    internal interface IApiService
    {
        Task<T> GetAsync<T>(string endpoint, string token = null, CancellationToken cancellationToken = default);

        Task<T> PostAsync<T>(string endpoint, object data, string token = null, CancellationToken cancellationToken = default);

        Task<T> PutAsync<T>(string endpoint, object data, string token = null, CancellationToken cancellationToken = default);
    }
}