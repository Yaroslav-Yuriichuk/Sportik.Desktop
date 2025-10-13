using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sportik.Desktop.Infrastructure.Services.Implementations
{
    public sealed class ExerciseSettingsService : IExerciseSettingsService
    {
        private readonly IExerciseSettingsRepository _exerciseSettingsRepository;
        private readonly IEventsService _eventsService;

        public ExerciseSettingsService(IExerciseSettingsRepository exerciseSettingsRepository, IEventsService eventsService)
        {
            _exerciseSettingsRepository = exerciseSettingsRepository;
            _eventsService = eventsService;
        }

        public async Task<IEnumerable<ExerciseSettings>> GetAllExerciseSettingsAsync(CancellationToken cancellationToken = default)
        {
            return await _exerciseSettingsRepository.GetAllAsync(cancellationToken);
        }

        public async Task<ExerciseSettings> GetExerciseSettingsAsync(Exercise exercise, CancellationToken cancellationToken)
        {
            return await _exerciseSettingsRepository.GetByKindAsync(exercise.Kind, cancellationToken);
        }

        public async Task<ExerciseSettings> UpdateExerciseSettingsAsync(ExerciseSettingsDelta exerciseSettingsDelta, Exercise exercise,
            CancellationToken cancellationToken)
        {
            ExerciseSettings exerciseSettings = await _exerciseSettingsRepository.GetByKindAsync(exercise.Kind, cancellationToken);

            if ((exerciseSettingsDelta.Change & ExerciseSettingsChange.IsEnabled) == ExerciseSettingsChange.IsEnabled)
            {
                exerciseSettings.IsEnabled = exerciseSettingsDelta.IsEnabled;
            }

            if ((exerciseSettingsDelta.Change & ExerciseSettingsChange.TargetRepetitions) == ExerciseSettingsChange.TargetRepetitions)
            {
                exerciseSettings.TargetRepetitions = exerciseSettingsDelta.TargetRepetitions;
            }

            if ((exerciseSettingsDelta.Change & ExerciseSettingsChange.TimeBetweenSets) == ExerciseSettingsChange.TimeBetweenSets)
            {
                exerciseSettings.TimeBetweenSets = exerciseSettingsDelta.TimeBetweenSets;
            }

            if ((exerciseSettingsDelta.Change & ExerciseSettingsChange.ExecutionTime) == ExerciseSettingsChange.ExecutionTime)
            {
                exerciseSettings.ExecutionTime = exerciseSettingsDelta.ExecutionTime;
            }

            exerciseSettings = await _exerciseSettingsRepository.UpdateAsync(exerciseSettings, cancellationToken);

            if ((exerciseSettingsDelta.Change & ExerciseSettingsChange.IsEnabled) == ExerciseSettingsChange.IsEnabled)
            {
                _eventsService.RaiseEvent(new ExerciseIsEnabledChangedEventArgs(exerciseSettings.Exercise, exerciseSettings.IsEnabled));
            }

            if ((exerciseSettingsDelta.Change & ExerciseSettingsChange.TimeBetweenSets) == ExerciseSettingsChange.TimeBetweenSets)
            {
                _eventsService.RaiseEvent(new ExerciseTimeBetweenSetsChangedEventArgs(exerciseSettings.Exercise, exerciseSettings.TimeBetweenSets));
            }

            if ((exerciseSettingsDelta.Change & ExerciseSettingsChange.ExecutionTime) == ExerciseSettingsChange.ExecutionTime)
            {
                _eventsService.RaiseEvent(new ExerciseExecutionTimeChangedEventArgs(exerciseSettings.Exercise, exerciseSettings.ExecutionTime));
            }

            return exerciseSettings;
        }
    }
}
