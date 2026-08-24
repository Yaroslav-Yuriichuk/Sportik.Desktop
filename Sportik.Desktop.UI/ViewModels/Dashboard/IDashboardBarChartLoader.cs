using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.UI.Models;

namespace Sportik.Desktop.UI.ViewModels.Dashboard
{
    internal interface IDashboardBarChartLoader
    {
        Task<OperationResult<IEnumerable<BarChartColumn>>> LoadBarChartAsync(CancellationToken cancellationToken);
    }
}