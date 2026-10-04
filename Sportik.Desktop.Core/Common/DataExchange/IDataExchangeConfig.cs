namespace Sportik.Desktop.Core.Common.DataExchange
{
    public interface IDataExchangeConfig
    {
        DataExchangeType Type { get; }

        DataExchangeOption Option { get; }

        string Description { get; }
    }
}