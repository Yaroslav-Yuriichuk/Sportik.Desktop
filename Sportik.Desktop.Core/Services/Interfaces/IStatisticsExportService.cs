using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Common.Export;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IStatisticsExportService
    {
        Task<OperationResult> ExportAsync(IStatisticsExporter exporter, CancellationToken cancellationToken = default);
    }
}
