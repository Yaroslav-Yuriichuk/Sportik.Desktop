using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Common.Synchronization;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    internal interface ISynchronizationService
    {
        Task<OperationResult> SyncAsync(ISynchronizer synchronizer, CancellationToken cancellationToken = default);
    }
}