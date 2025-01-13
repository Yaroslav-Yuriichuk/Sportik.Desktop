using System;

namespace Sportik.Core
{
    internal interface ITimer
    {
        TimeSpan Interval { get; }

        DateTime StartTime { get; }

        DateTime PauseTime { get; }

        DateTime ResumeTime { get; }

        DateTime StopTime { get; }

        TimeSpan ElapsedTime { get; }

        bool IsRunning { get; }

        bool IsPaused { get; }

        bool Loop { get; set; }

        void Start();

        void Pause();

        void Resume();

        void Stop();
    }
}
