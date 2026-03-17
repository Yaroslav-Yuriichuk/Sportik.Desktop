using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IAuthService
    {
        Task<OperationResult<string>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

        Task<OperationResult<string>> GetTokenAsync(CancellationToken cancellationToken = default);

        Task<OperationResult<Guid>> GetUserIdAsync(CancellationToken cancellationToken = default);

        Task<OperationResult<string>> GetEmailAsync(CancellationToken cancellationToken = default);

        Task<OperationResult> LogoutAsync(CancellationToken cancellationToken = default);
    }
}