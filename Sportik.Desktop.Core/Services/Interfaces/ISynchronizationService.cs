using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    internal interface ISynchronizationService
    {
        Task<OperationResult> SyncAsync(SyncOption option, CancellationToken cancellationToken = default);
    }
}