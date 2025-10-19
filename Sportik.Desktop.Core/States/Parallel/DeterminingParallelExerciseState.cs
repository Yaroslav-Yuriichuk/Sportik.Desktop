using System;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.States.Parallel
{
    internal sealed class DeterminingParallelExerciseState : ParallelExerciseState
    {
        private readonly Func<IExercisesService> _exercisesServiceFactory;
        public override States.ParallelExerciseState ExerciseState => States.ParallelExerciseState.Unknown;

        public DeterminingParallelExerciseState(ParallelExerciseStatesContext context,
            Func<IExercisesService> exercisesServiceFactory): base(context)
        {
            _exercisesServiceFactory = exercisesServiceFactory;
        }

        protected override void HandleEnter()
        {
            IExercisesService exercisesService = _exercisesServiceFactory();

            Task.Run(async () =>
            {
                Exercise exercise = await exercisesService.GetByIdAsync(Context.ExerciseId, ActiveCancellationToken);

                ParallelExerciseState state = exercise.Settings.IsEnabled
                    ? Context.WaitingBeforeForceExecutionExerciseState
                    : Context.DisabledExerciseState;

                Context.Switch(state);
            });
        }

        protected override void HandleExit() { }
    }
}