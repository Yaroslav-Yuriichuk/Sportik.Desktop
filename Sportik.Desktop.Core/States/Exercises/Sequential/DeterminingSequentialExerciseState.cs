using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                Exercise exercise = await exercisesService.GetByIdAsync(Context.ExerciseId, ActiveCancellationToken);

                IEnumerable<Exercise> exercises =
                    await exercisesService.GetByIdsAsync(Context.ExerciseIds, ActiveCancellationToken);

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