using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Media.Core;
using System.Threading;
using Sportik.Desktop.Sound.Models;
using Sportik.Desktop.Sound.Services.Interfaces;

namespace Sportik.Desktop.Sound.Services.Implementations
{
    public sealed class SoundService : ISoundService
    {
        public async Task PlayAsync(SoundSource soundSource, CancellationToken cancellationToken = default)
        {
            MediaPlayer player = new MediaPlayer();
            
            MediaSource mediaSource = await CreateMediaSourceAsync(soundSource, cancellationToken);
            player.Source = mediaSource;

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            cancellationToken.Register(() =>
            {
                player.Pause();
                player.Source = null;
                tcs.TrySetResult(true);
            });

            player.MediaEnded += (sender, args) =>
            {
                tcs.TrySetResult(true);
            };

            player.MediaFailed += (sender, args) =>
            {
                tcs.TrySetResult(true);
            };

            player.Play();

            await Task.Run(() => tcs.Task, cancellationToken);
        }

        private async Task<MediaSource> CreateMediaSourceAsync(SoundSource soundSource, CancellationToken cancellationToken)
        {
            if (soundSource.IsSystem)
            {
                return MediaSource.CreateFromUri(soundSource.Uri);
            }

            StorageFile file;

            try
            {
                file = await Task.Run(async () => await StorageFile.GetFileFromApplicationUriAsync(soundSource.Uri), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException(cancellationToken);
            }
            catch (Exception)
            {
                return null;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            return MediaSource.CreateFromStorageFile(file);
        }
    }
}
