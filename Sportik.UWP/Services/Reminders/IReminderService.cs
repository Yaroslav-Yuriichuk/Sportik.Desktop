using System.Collections.Generic;
using Sportik.UWP.Models;

namespace Sportik.UWP.Services.Reminders
{
    internal interface IReminderService
    {
        void Start(IEnumerable<Exercise> exercises);

        void Stop();

        ExerciseStateKind GetExerciseState(Exercise exercise);
    }
}
