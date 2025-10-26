using System;

namespace Sportik.Desktop.Core.Models.Sound
{
    public sealed class SoundSource
    {
        public Uri Uri { get; }

        public bool IsSystem { get; }

        private SoundSource(Uri uri, bool isSystem)
        {
            Uri = uri;
            IsSystem = isSystem;
        }

        public static SoundSource System(SystemSound sound)
        {
            return sound switch
            {
                SystemSound.Notification => new SoundSource(new Uri("ms-winsoundevent://Notification.Default"), true),
                _ => throw new ArgumentOutOfRangeException(nameof(sound), sound, null)
            };
        }

        public static SoundSource Custom(string path)
        {
            return new SoundSource(new Uri($"ms-appx:///{path}"), false);
        }
    }
}
