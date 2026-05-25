using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Common.Import;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IStatisticsImportService
    {
        Task<OperationResult> ImportAsync(IStatisticsImporter importer, CancellationToken cancellationToken = default);
    }
}