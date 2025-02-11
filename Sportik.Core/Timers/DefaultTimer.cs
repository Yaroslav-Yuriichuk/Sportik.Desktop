using System;
using System.Timers;

namespace Sportik.Core.Timers
{
    internal sealed class DefaultTimer : ITimer, IDisposable
    {
        public TimeSpan Interval
        {
            get => _originalInterval;
            set
            {
                TimeSpan elapsedTime = ElapsedTime;

                if (IsRunning)
                {
                    _timer.Stop();
                    _originalInterval = value;

                    _timer.Interval = elapsedTime < value
                        ? value.TotalMilliseconds - elapsedTime.TotalMilliseconds
                        : value.TotalMilliseconds;

                    if (elapsedTime < value || Loop)
                    {
                        _timer.Start();
                    }

                    if (elapsedTime >= value)
                    {
                        _callback?.Invoke(this, EventArgs.Empty);
                        Elapsed?.Invoke(this, EventArgs.Empty);
                    }

                    return;
                }

                if (IsPaused)
                {
                    _originalInterval = value;

                    if (elapsedTime >= value)
                    {
                        _callback?.Invoke(this, EventArgs.Empty);
                        Elapsed?.Invoke(this, EventArgs.Empty);
                    }

                    return;
                }

                _originalInterval = value;
            }
        }

        public TimeSpan ElapsedTime => _accumulatedElapsedTime + (IsRunning ? DateTime.Now - _lastStartTime : TimeSpan.Zero);

        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }

        public bool Loop
        {
            get => _timer.AutoReset;
            set => _timer.AutoReset = value;
        }

        public event EventHandler Elapsed;

        private readonly Timer _timer;
        private readonly EventHandler _callback;

        private TimeSpan _originalInterval;
        private DateTime _lastStartTime;
        private TimeSpan _accumulatedElapsedTime;

        public DefaultTimer(TimeSpan interval, EventHandler callback = null)
        {
            _timer = new Timer(interval.TotalMilliseconds);
            _timer.Elapsed += InvokeCallback;

            _callback = callback;
            _originalInterval = interval;

            Reset();
        }

        public void Dispose()
        {
            Reset();

            _timer.Elapsed -= InvokeCallback;
            _timer.Dispose();
        }
        
        public void Start()
        {
            if (IsPaused)
            {
                Reset();

                _lastStartTime = DateTime.Now;

                IsRunning = true;
                IsPaused = false;

                _timer.Interval = _originalInterval.TotalMilliseconds;
                _timer.Start();

                return;
            }

            if (!IsRunning)
            {
                _lastStartTime = DateTime.Now;

                IsRunning = true;
                IsPaused = false;

                _timer.Interval = _originalInterval.TotalMilliseconds;

                _timer.Start();
            }
        }

        public void Pause()
        {
            if (IsRunning && !IsPaused)
            {
                _accumulatedElapsedTime += DateTime.Now - _lastStartTime;

                IsRunning = false;
                IsPaused = true;

                _timer.Stop();
            }
        }

        public void Resume()
        {
            if (!IsRunning && IsPaused)
            {
                _lastStartTime = DateTime.Now;

                IsRunning = true;
                IsPaused = false;

                TimeSpan interval = Interval - ElapsedTime;
                _timer.Interval = interval.TotalMilliseconds;

                _timer.Start();
            }
        }

        public void Stop()
        {
            if (IsPaused)
            {
                IsRunning = false;
                IsPaused = false;

                _timer.Stop();
                Reset();

                return;
            }

            if (IsRunning)
            {
                IsRunning = false;
                IsPaused = false;

                _timer.Stop();
                Reset();
            }
        }

        public void Reset()
        {
            if (!IsRunning)
            {
                IsRunning = false;
                IsPaused = false;

                _accumulatedElapsedTime = TimeSpan.Zero;
                _lastStartTime = DateTime.MinValue;
            }
        }

        private void InvokeCallback(object sender, ElapsedEventArgs e)
        {
            if (Loop)
            {
                _timer.Interval = _originalInterval.TotalMilliseconds;
            }
            else
            {
                Stop();
            }
            
            _callback?.Invoke(this, EventArgs.Empty);
            Elapsed?.Invoke(this, EventArgs.Empty);
        }
    }
}
