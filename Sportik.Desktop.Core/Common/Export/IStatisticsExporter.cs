using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Repositories.Interfaces;

namespace Sportik.Desktop.Core.Common.Export
{
    public interface IStatisticsExporter
    {
        internal void Initialize(
            IExercisesRepository exercisesRepository,
            IExerciseStatisticsRepository exerciseStatisticsRepository);

        Task ExportAsync(CancellationToken cancellationToken);
    }
}
