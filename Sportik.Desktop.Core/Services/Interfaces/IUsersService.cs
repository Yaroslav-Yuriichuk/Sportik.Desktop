using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IUsersService
    {
        Task<OperationResult<Guid>> RegisterAsync(string email, string password, CancellationToken cancellationToken);
    }
}