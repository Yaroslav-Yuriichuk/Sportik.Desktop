using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Common
{
    public interface IDataExchanger
    {
        internal void Initialize(
            IExercisesRepository exercisesRepository,
            IExerciseStatisticsRepository exerciseStatisticsRepository,
            IEventsService eventsService);

        Task ExchangeAsync(CancellationToken cancellationToken);
    }
}