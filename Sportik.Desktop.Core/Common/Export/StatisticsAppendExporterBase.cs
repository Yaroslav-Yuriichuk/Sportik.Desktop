using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common.DataExchange;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Goal;
using Sportik.Desktop.Core.Models.ImportExport;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Common.Export
{
    public abstract class StatisticsAppendExporterBase : IDataExchanger
    {
        private IExercisesRepository _exercisesRepository;
        private IExerciseStatisticsRepository _exerciseStatisticsRepository;
        private IWeekGoalsRepository _weekGoalsRepository;

        void IDataExchanger.Initialize(
            IExercisesRepository exercisesRepository,
            IExerciseStatisticsRepository exerciseStatisticsRepository,
            IWeekGoalsRepository weekGoalsRepository,
            IEventsService eventsService)
        {
            _exercisesRepository = exercisesRepository;
            _exerciseStatisticsRepository = exerciseStatisticsRepository;
            _weekGoalsRepository = weekGoalsRepository;
        }

        public async Task ExchangeAsync(DataExchangeContext context, CancellationToken cancellationToken)
        {
            if (_exercisesRepository is null || _exerciseStatisticsRepository is null || _weekGoalsRepository is null)
            {
                throw new InvalidOperationException("Exporter is not initialized.");
            }

            IList<Exercise> exercises = await GetExercisesAsync(context, cancellationToken);
            IList<ExerciseSet> exerciseSets = await GetSetsAsync(context, cancellationToken);
            IList<WeekGoal> weekGoals = await GetWeekGoalsAsync(context, cancellationToken);

            Dictionary<Guid, string> exerciseNamesById = exercises
                .ToDictionary(exercise => exercise.Id, exercise => exercise.Name);

            List<Task> exportTasks = new List<Task>();

            if (context.Scope.HasFlag(DataExchangeScope.Exercises) && ToExportExercises())
            {
                List<ExportExercise> exportExercises = exercises
                    .Where(exercise => !string.IsNullOrWhiteSpace(exercise.Name))
                    .Select(exercise => new ExportExercise(
                        exercise.Name,
                        exercise.Settings.TargetRepetitions,
                        exercise.Settings.TimeBetweenSets,
                        exercise.Settings.ExecutionTime))
                    .ToList();

                exportTasks.Add(AppendExercisesAsync(exportExercises, cancellationToken));
            }

            if (context.Scope.HasFlag(DataExchangeScope.Sets) && ToExportSets())
            {
                List<ExportSet> exportSets = exerciseSets
                    .OrderBy(set => set.LoggedAt)
                    .Where(set => exerciseNamesById.TryGetValue(set.ExerciseId, out string exerciseName) && !string.IsNullOrWhiteSpace(exerciseName))
                    .Select(set => new ExportSet(
                        exerciseNamesById[set.ExerciseId],
                        set.LoggedAt.ToUniversalTime(),
                        set.Repetitions))
                    .ToList();

                exportTasks.Add(AppendSetsAsync(exportSets, cancellationToken));
            }

            if (context.Scope.HasFlag(DataExchangeScope.WeekGoals) && ToExportWeekGoals())
            {
                List<ExportExerciseGoal> exportWeekGoals = weekGoals
                    .SelectMany(weekGoal => weekGoal.ExerciseGoals.Select(exerciseGoal => new ExportExerciseGoal(
                        exerciseGoal.Exercise.Name,
                        weekGoal.FirstWeekDayDate,
                        exerciseGoal.TargetRepetitions)))
                    .ToList();

                exportTasks.Add(AppendWeekGoalsAsync(exportWeekGoals, cancellationToken));
            }

            await Task.WhenAll(exportTasks);
        }

        private async Task<IList<Exercise>> GetExercisesAsync(DataExchangeContext context, CancellationToken cancellationToken)
        {
            if (context?.Exercises is not null)
            {
                return context.Exercises.ToList();
            }

            return (await _exercisesRepository.GetAllAsync(cancellationToken)).ToList();
        }

        private async Task<IList<ExerciseSet>> GetSetsAsync(DataExchangeContext context, CancellationToken cancellationToken)
        {
            if (context?.Sets is not null)
            {
                return context.Sets.ToList();
            }

            return (await _exerciseStatisticsRepository.GetAllAsync(cancellationToken)).ToList();
        }

        private async Task<IList<WeekGoal>> GetWeekGoalsAsync(DataExchangeContext context, CancellationToken cancellationToken)
        {
            if (context?.WeekGoals is not null)
            {
                return context.WeekGoals.ToList();
            }

            return (await _weekGoalsRepository.GetAllAsync(cancellationToken)).ToList();
        }

        protected abstract bool ToExportExercises();

        protected abstract bool ToExportSets();

        protected abstract bool ToExportWeekGoals();

        protected abstract Task AppendExercisesAsync(IList<ExportExercise> exercises, CancellationToken cancellationToken);

        protected abstract Task AppendSetsAsync(IList<ExportSet> sets, CancellationToken cancellationToken);

        protected abstract Task AppendWeekGoalsAsync(IList<ExportExerciseGoal> weekGoals, CancellationToken cancellationToken);
    }
}
