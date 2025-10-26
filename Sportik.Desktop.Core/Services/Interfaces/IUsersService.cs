using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Backend.Domain.Common;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IUsersService
    {
        Task<OperationResult<Guid>> RegisterAsync(string email, string password, CancellationToken cancellationToken);
    }
}