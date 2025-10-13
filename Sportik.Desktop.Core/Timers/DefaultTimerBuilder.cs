using System;

namespace Sportik.Desktop.Core.Timers
{
    public class DefaultTimerBuilder
    {
        private TimeSpan _interval;
        private EventHandler _callback;
        private bool _loop;

        public DefaultTimerBuilder SetInterval(TimeSpan interval)
        {
            _interval = interval;
            return this;
        }

        public DefaultTimerBuilder SetCallback(EventHandler callback)
        {
            _callback = callback;
            return this;
        }

        public DefaultTimerBuilder SetLoop()
        {
            _loop = true;
            return this;
        }

        public ITimer Build()
        {
            return new DefaultTimer(_interval, _callback)
            {
                Loop = _loop,
            };
        }
    }
}
