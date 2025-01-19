using System.Collections.Generic;
using Sportik.Models;

namespace Sportik.Services.Reminders
{
    internal interface IReminderService
    {
        void Start(IEnumerable<Exercise> exercises);

        void Stop();

        ExerciseStateKind GetExerciseState(Exercise exercise);
    }
}
