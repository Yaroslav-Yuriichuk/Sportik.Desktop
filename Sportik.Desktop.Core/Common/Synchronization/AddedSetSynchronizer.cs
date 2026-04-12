using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Common.Synchronization
{
    internal sealed class AddedSetSynchronizer : Synchronizer
    {
        private readonly ExerciseSet _set;

        public AddedSetSynchronizer(ExerciseSet set)
        {
            _set = set;
        }

        public override async Task SyncAsync(CancellationToken cancellationToken)
        {
            AddExerciseSetModel addModel = new AddExerciseSetModel(_set.Id, _set.Repetitions, _set.LoggedAt, _set.ExerciseId);
            await LocalExerciseStatisticsRepository.AddSetAsync(addModel, cancellationToken);
        }
    }
}