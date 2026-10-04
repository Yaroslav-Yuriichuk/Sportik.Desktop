using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IDataExchangeService
    {
        Task<OperationResult> ExchangeAsync(IDataExchanger exchanger, CancellationToken cancellationToken = default);
    }
}