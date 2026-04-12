using System.Threading;
using System.Threading.Tasks;

namespace Sportik.Desktop.Core.Common.Synchronization
{
    internal sealed class DeleteAllSynchronizer : Synchronizer
    {
        public override async Task SyncAsync(CancellationToken cancellationToken)
        {
            await LocalExercisesRepository.DeleteAllAsync(cancellationToken);
        }
    }
}