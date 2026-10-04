using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Common.DataExchange;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IDataExchangeService
    {
        Task<OperationResult> ExchangeAsync(IDataExchanger exchanger, CancellationToken cancellationToken = default);
    }
}