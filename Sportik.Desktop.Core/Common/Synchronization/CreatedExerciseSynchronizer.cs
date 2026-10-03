using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;

namespace Sportik.Desktop.Core.Common.Synchronization
{
    internal sealed class CreatedExerciseSynchronizer : Synchronizer
    {
        private readonly Exercise _exercise;

        public CreatedExerciseSynchronizer(Exercise exercise)
        {
            _exercise = exercise;
        }

        public override async Task SyncAsync(CancellationToken cancellationToken)
        {
            AddExerciseModel addModel = new AddExerciseModel(_exercise.Id, _exercise.Name, new AddExerciseSettingsModel(_exercise.Settings));
            await LocalExercisesRepository.AddAsync(addModel, cancellationToken);
        }
    }
}