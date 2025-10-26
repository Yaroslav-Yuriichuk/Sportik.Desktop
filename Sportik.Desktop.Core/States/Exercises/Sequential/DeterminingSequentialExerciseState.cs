using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sportik.Backend.Domain.Common;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.Exercises.Sequential
{
    internal sealed class DeterminingSequentialExerciseState : SequentialExerciseState
    {
        private readonly Func<IExercisesService> _exercisesServiceFactory;
        private readonly IRuntimeCacheService _runtimeCacheService;

        public override Exercises.SequentialExerciseState ExerciseState => Exercises.SequentialExerciseState.Unknown;

        public DeterminingSequentialExerciseState(SequentialExercisesStatesContext context,
            Func<IExercisesService> exercisesServiceFactory, IRuntimeCacheService runtimeCacheService) : base(context)
        {
            _exercisesServiceFactory = exercisesServiceFactory;
            _runtimeCacheService = runtimeCacheService;
        }

        protected override void HandleEnter()
        {
            IExercisesService exercisesService = _exercisesServiceFactory();

            Task.Run(async () =>
            {
                OperationResult<Exercise> exerciseResult = await exercisesService.GetByIdAsync(Context.ExerciseId, ActiveCancellationToken);

                OperationResult<IEnumerable<Exercise>> exercisesResult =
                    await exercisesService.GetByIdsAsync(Context.ExerciseIds, ActiveCancellationToken);

                if (!exerciseResult.Succeeded || !exercisesResult.Succeeded)
                {
                    // TODO: Handle error.
                    return;
                }

                Exercise exercise = exerciseResult.Value;
                IEnumerable<Exercise> exercises = exercisesResult.Value;

                if (exercise.Settings.IsEnabled)
                {
                    SequentialExerciseState state;

                    if (_runtimeCacheService.TryGet(out SequentialExercisesCache sequentialExercisesCache))
                    {
                        state = Context.ExerciseId == sequentialExercisesCache.LastActiveExerciseId
                            ? Context.WaitingBeforeForceExecutionExerciseState
                            : Context.QueuedExerciseState;
                    }
                    else
                    {
                        Guid firstEnabledExerciseId = exercises.First(e => e.Settings.IsEnabled).Id;

                        state = Context.ExerciseId == firstEnabledExerciseId
                            ? Context.WaitingBeforeForceExecutionExerciseState
                            : Context.QueuedExerciseState;
                    }

                    Context.Switch(state);
                }
                else
                {
                    Context.Switch(Context.DisabledExerciseState);
                }
            });
        }

        protected override void HandleExit() { }
    }
}