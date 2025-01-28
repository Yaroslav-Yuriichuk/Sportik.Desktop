using System.Collections.Generic;
using System.Linq;
using Sportik.UWP.Models;
using Sportik.UWP.Services.Reminders.States;

namespace Sportik.UWP.Services.Reminders
{
    internal sealed class ReminderService : IReminderService
    {
        private IEnumerable<ExerciseStatesContext> _contexts;

        public void Start(IEnumerable<Exercise> exercises)
        {
            if (_contexts != null)
            {
                return;
            }

            _contexts = exercises
                .Select(exercise => new ExerciseStatesContext(exercise))
                .ToArray();
        }

        public void Stop()
        {
            if (_contexts == null)
            {
                return;
            }

            foreach (ExerciseStatesContext context in _contexts)
            {
                context.Dispose();
            }

            _contexts = null;
        }

        public ExerciseStateKind GetExerciseState(Exercise exercise)
        {
            if (_contexts == null)
            {
                return ExerciseStateKind.Unknown;
            }

            ExerciseStatesContext context = _contexts.FirstOrDefault(c => c.Exercise.Id == exercise.Id);

            return context?.CurrentState.Kind ?? ExerciseStateKind.Unknown;
        }
    }
}
