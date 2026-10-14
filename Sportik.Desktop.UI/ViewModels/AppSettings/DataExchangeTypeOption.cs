using Sportik.Desktop.Core.Common.DataExchange;

namespace Sportik.Desktop.UI.ViewModels.AppSettings
{
    internal sealed class DataExchangeTypeOption
    {
        public string Name { get; }

        public DataExchangeType Type { get; }

        public DataExchangeTypeOption(string name, DataExchangeType type)
        {
            Name = name;
            Type = type;
        }
    }
}