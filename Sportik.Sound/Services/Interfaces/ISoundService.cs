using System.Threading;
using System.Threading.Tasks;
using Sportik.Sound.Models;

namespace Sportik.Sound.Services.Interfaces
{
    public interface ISoundService
    {
        Task PlayAsync(SoundSource soundSource, CancellationToken cancellationToken = default);
    }
}
