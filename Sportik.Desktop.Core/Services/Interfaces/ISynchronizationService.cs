using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    internal interface ISynchronizationService
    {
        Task<OperationResult> SyncAsync(CancellationToken cancellationToken = default);
    }
}