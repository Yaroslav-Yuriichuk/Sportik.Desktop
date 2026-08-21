using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Repositories.Interfaces;

namespace Sportik.Desktop.Core.Common.Export
{
    public abstract class StatisticsExporterBase : IStatisticsExporter
    {
        private IExercisesRepository _exercisesRepository;
        private IExerciseStatisticsRepository _exerciseStatisticsRepository;

        void IStatisticsExporter.Initialize(IExercisesRepository exercisesRepository,
            IExerciseStatisticsRepository exerciseStatisticsRepository)
        {
            _exercisesRepository = exercisesRepository;
            _exerciseStatisticsRepository = exerciseStatisticsRepository;
        }

        public async Task ExportAsync(CancellationToken cancellationToken)
        {
            if (_exercisesRepository is null || _exerciseStatisticsRepository is null)
            {
                throw new InvalidOperationException("Exporter is not initialized.");
            }

            IEnumerable<Exercise> exercises = await _exercisesRepository.GetAllAsync(cancellationToken);

            Dictionary<Guid, string> exerciseNamesById = exercises
                .ToDictionary(exercise => exercise.Id, exercise => exercise.Name);

            IEnumerable<ExerciseSet> exerciseSets = await _exerciseStatisticsRepository.GetAllAsync(cancellationToken);

            List<ExportExercise> exportExercises = exerciseSets
                .OrderBy(set => set.LoggedAt)
                .Where(set => exerciseNamesById.TryGetValue(set.ExerciseId, out string exerciseName) && !string.IsNullOrWhiteSpace(exerciseName))
                .Select(set => new ExportExercise(
                    exerciseNamesById[set.ExerciseId],
                    set.LoggedAt.ToUniversalTime(),
                    set.Repetitions))
                .ToList();

            await WriteExercisesAsync(exportExercises, cancellationToken);
        }

        protected abstract Task WriteExercisesAsync(IList<ExportExercise> exercises, CancellationToken cancellationToken);
    }
}