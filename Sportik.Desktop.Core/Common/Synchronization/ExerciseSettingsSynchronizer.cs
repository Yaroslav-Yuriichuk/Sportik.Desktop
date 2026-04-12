using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;

namespace Sportik.Desktop.Core.Common.Synchronization
{
    internal sealed class ExerciseSettingsSynchronizer : Synchronizer
    {
        private readonly Guid _exerciseId;
        private readonly ExerciseSettingsDelta _delta;

        public ExerciseSettingsSynchronizer(Guid exerciseId, ExerciseSettingsDelta delta)
        {
            _exerciseId = exerciseId;
            _delta = delta;
        }

        public override async Task SyncAsync(CancellationToken cancellationToken)
        {
            UpdateExerciseSettingsModel updateModel = new UpdateExerciseSettingsModel(_exerciseId, _delta);
            await LocalExerciseSettingsRepository.UpdateAsync(updateModel, cancellationToken);
        }
    }
}