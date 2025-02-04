using System.Collections.Generic;
using Sportik.Automation.States;
using Sportik.Core.Models;

namespace Sportik.Automation.Services
{
    public interface IReminderService
    {
        void Start(IEnumerable<Exercise> exercises);

        void Stop();

        ExerciseStateKind GetExerciseState(Exercise exercise);
    }
}
