using System;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Common;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.Exercises.Parallel
{
    internal sealed class DeterminingParallelExerciseState : ParallelExerciseState
    {
        private readonly Func<IExercisesService> _exercisesServiceFactory;

        public override Exercises.ParallelExerciseState ExerciseState => Exercises.ParallelExerciseState.Unknown;

        public DeterminingParallelExerciseState(ParallelExerciseStatesContext context,
            Func<IExercisesService> exercisesServiceFactory) : base(context)
        {
            _exercisesServiceFactory = exercisesServiceFactory;
        }

        protected override void HandleEnter()
        {
            IExercisesService exercisesService = _exercisesServiceFactory();

            Task.Run(async () =>
            {
                OperationResult<Exercise> result = await exercisesService.GetByIdAsync(Context.ExerciseId, ActiveCancellationToken);

                if (!result.Succeeded)
                {
                    // TODO: Handle error.
                    return;
                }

                ParallelExerciseState state = result.Value.Settings.IsEnabled
                    ? Context.WaitingBeforeForceExecutionExerciseState
                    : Context.DisabledExerciseState;

                Context.Switch(state);
            });
        }

        protected override void HandleExit() { }
    }
}