using System;
using System.Timers;

namespace Sportik.Core
{
    internal sealed class DefaultTimer : ITimer, IDisposable
    {
        public DateTime StartTime { get; private set; } = DateTime.MinValue;
        public DateTime PauseTime { get; private set; } = DateTime.MinValue;
        public DateTime ResumeTime { get; private set; } = DateTime.MinValue;
        public DateTime StopTime { get; private set; } = DateTime.MinValue;
        
        public TimeSpan Interval { get; }
        public TimeSpan ElapsedTime => _accumulatedElapsedTime + (IsRunning ? DateTime.Now - _lastStartTime : TimeSpan.Zero);

        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }

        public bool Loop
        {
            get => _timer.AutoReset;
            set => _timer.AutoReset = value;
        }

        private readonly Timer _timer;
        private readonly EventHandler _callback;

        private DateTime _lastStartTime;
        private TimeSpan _accumulatedElapsedTime;

        public DefaultTimer(TimeSpan interval, EventHandler callback = null)
        {
            _timer = new Timer(interval.TotalMilliseconds);
            _timer.Elapsed += InvokeCallback;

            Interval = interval;
            _callback = callback;
        }

        public void Dispose()
        {
            _timer.Elapsed -= InvokeCallback;
            _timer.Dispose();
        }
        
        public void Start()
        {
            if (!IsRunning && !IsPaused)
            {
                StartTime = DateTime.Now;
                _lastStartTime = StartTime;

                IsRunning = true;

                _timer.Start();
            }
        }

        public void Pause()
        {
            if (IsRunning && !IsPaused)
            {
                PauseTime = DateTime.Now;
                _accumulatedElapsedTime += PauseTime - _lastStartTime;

                IsRunning = false;
                IsPaused = true;

                _timer.Stop();
            }
        }

        public void Resume()
        {
            if (!IsRunning && IsPaused)
            {
                ResumeTime = DateTime.Now;
                _lastStartTime = ResumeTime;

                IsRunning = true;
                IsPaused = false;

                TimeSpan interval = Interval - ElapsedTime;
                _timer.Interval = interval.TotalMilliseconds;

                _timer.Start();
            }
        }

        public void Stop()
        {
            if (IsRunning && !IsPaused)
            {
                StopTime = DateTime.Now;
                IsRunning = false;

                _timer.Stop();
            }
        }

        private void InvokeCallback(object sender, ElapsedEventArgs e)
        {
            if (Loop)
            {
                _timer.Interval = Interval.TotalMilliseconds;
            }
            else
            {
                Stop();
            }

            _callback?.Invoke(this, null);
        }
    }
}
