using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Sound.Models;

namespace Sportik.Desktop.Sound.Services.Interfaces
{
    public interface ISoundService
    {
        Task PlayAsync(SoundSource soundSource, CancellationToken cancellationToken = default);
    }
}
