using System;
using Sportik.Desktop.Core.Common.DataExchange;

namespace Sportik.Desktop.Core.Events
{
    public sealed class DataExchangeConfigEditedEventArgs : EventArgs
    {
        public IDataExchangeConfig PreviousConfig { get; }

        public IDataExchangeConfig CurrentConfig { get; }

        public DataExchangeConfigEditedEventArgs(IDataExchangeConfig previousConfig, IDataExchangeConfig currentConfig)
        {
            PreviousConfig = previousConfig;
            CurrentConfig = currentConfig;
        }
    }
}