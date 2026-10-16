using System;
using Sportik.Desktop.Core.Common.DataExchange;

namespace Sportik.Desktop.Core.Events
{
    public sealed class DataExchangeConfigRemovedEventArgs : EventArgs
    {
        public IDataExchangeConfig Config { get; }

        public DataExchangeConfigRemovedEventArgs(IDataExchangeConfig config)
        {
            Config = config;
        }
    }
}