using System;
using Sportik.Desktop.Core.Common.DataExchange;

namespace Sportik.Desktop.Core.Events
{
    public sealed class DataExchangeConfigAddedEventArgs : EventArgs
    {
        public IDataExchangeConfig DataExchangeConfig { get; }

        public DataExchangeConfigAddedEventArgs(IDataExchangeConfig dataExchangeConfig)
        {
            DataExchangeConfig = dataExchangeConfig;
        }
    }
}