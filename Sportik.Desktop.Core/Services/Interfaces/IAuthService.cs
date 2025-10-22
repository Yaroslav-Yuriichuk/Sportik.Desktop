using System.Threading;
using System.Threading.Tasks;
using Sportik.Backend.Domain.Common;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IAuthService
    {
        Task<OperationResult<string>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

        Task<OperationResult<string>> GetTokenAsync(CancellationToken cancellationToken = default);

        Task<OperationResult> LogoutAsync(CancellationToken cancellationToken = default);
    }
}