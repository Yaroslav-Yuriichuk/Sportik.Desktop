using System;

namespace Sportik.UWP.Core
{
    public interface ITimer
    {
        TimeSpan Interval { get; set; }

        TimeSpan ElapsedTime { get; }

        bool IsRunning { get; }

        bool IsPaused { get; }

        bool Loop { get; set; }

        event EventHandler Elapsed;

        void Start();

        void Pause();

        void Resume();

        void Stop();

        void Reset();
    }
}
