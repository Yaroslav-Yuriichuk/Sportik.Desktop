using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Models;
using Sportik.Models.Settings;

namespace Sportik.Services.Settings
{
    internal sealed class ExerciseSettingsService : IExerciseSettingsService
    {
        private readonly IExerciseSettingsRepository _exerciseSettingsRepository;

        public ExerciseSettingsService(IExerciseSettingsRepository exerciseSettingsRepository)
        {
            _exerciseSettingsRepository = exerciseSettingsRepository;
        }

        public async Task<IEnumerable<ExerciseSettings>> GetExerciseSettingsAsync(CancellationToken cancellationToken = default)
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

            if (exerciseSettings == null)
            {
                exerciseSettings = new ExerciseSettings
                {
                    Exercise = exercise,
                    IsEnabled = exerciseSettingsDelta.IsEnabled,
                    TargetRepetitions = exerciseSettingsDelta.TargetRepetitions,
                    TimeBetweenSets = exerciseSettingsDelta.TimeBetweenSets,
                    ExecutionTime = exerciseSettingsDelta.ExecutionTime,
                };

                return await _exerciseSettingsRepository.AddAsync(exerciseSettings, cancellationToken);
            }

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

            return await _exerciseSettingsRepository.UpdateAsync(exerciseSettings, cancellationToken);
        }
    }
}
