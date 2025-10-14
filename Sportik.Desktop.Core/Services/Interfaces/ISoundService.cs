using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models.Sound;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface ISoundService
    {
        Task PlayAsync(SoundSource soundSource, CancellationToken cancellationToken = default);
    }
}
