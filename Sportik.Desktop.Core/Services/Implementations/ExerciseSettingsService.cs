using System;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Backend.Domain.Common;
using Sportik.Desktop.Core.Events;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Settings;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Services.Implementations
{
    internal sealed class ExerciseSettingsService : IExerciseSettingsService
    {
        private readonly IExerciseSettingsRepository _exerciseSettingsRepository;
        private readonly IEventsService _eventsService;

        public ExerciseSettingsService(Func<DataSource, IExerciseSettingsRepository> exerciseSettingsRepositoryFactory,
            IEventsService eventsService)
        {
            _exerciseSettingsRepository = exerciseSettingsRepositoryFactory(DataSource.Remote);
            _eventsService = eventsService;
        }

        public async Task<OperationResult<ExerciseSettings>> UpdateAsync(ExerciseSettingsDelta delta, Guid exerciseId,
            CancellationToken cancellationToken)
        {
            try
            {
                ExerciseSettings updatedSettings =
                    await _exerciseSettingsRepository.UpdateAsync(delta, exerciseId, cancellationToken);

                ExerciseSettingsChange change = delta.Change;

                if (change.HasFlag(ExerciseSettingsChange.IsEnabled))
                {
                    _eventsService.RaiseEvent(new ExerciseIsEnabledChangedEventArgs(exerciseId, updatedSettings.IsEnabled));
                }

                if (change.HasFlag(ExerciseSettingsChange.TimeBetweenSets))
                {
                    _eventsService.RaiseEvent(new ExerciseTimeBetweenSetsChangedEventArgs(exerciseId, updatedSettings.TimeBetweenSets));
                }

                if (change.HasFlag(ExerciseSettingsChange.ExecutionTime))
                {
                    _eventsService.RaiseEvent(new ExerciseExecutionTimeChangedEventArgs(exerciseId, updatedSettings.ExecutionTime));
                }

                return OperationResult<ExerciseSettings>.Success(updatedSettings);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult<ExerciseSettings>.Failure(new[] { "Failed to update exercise settings." });
            }
        }
    }
}
