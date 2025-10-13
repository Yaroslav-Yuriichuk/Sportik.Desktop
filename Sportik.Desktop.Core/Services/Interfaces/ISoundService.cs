using System.Threading;
using System.Threading.Tasks;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface ISoundService
    {
        Task PlayAsync(SoundSource soundSource, CancellationToken cancellationToken = default);
    }
}
