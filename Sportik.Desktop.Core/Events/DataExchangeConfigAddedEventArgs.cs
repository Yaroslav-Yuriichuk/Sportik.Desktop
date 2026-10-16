using System;
using Sportik.Desktop.Core.Common.DataExchange;

namespace Sportik.Desktop.Core.Events
{
    public sealed class DataExchangeConfigAddedEventArgs : EventArgs
    {
        public IDataExchangeConfig Config { get; }

        public DataExchangeConfigAddedEventArgs(IDataExchangeConfig config)
        {
            Config = config;
        }
    }
}