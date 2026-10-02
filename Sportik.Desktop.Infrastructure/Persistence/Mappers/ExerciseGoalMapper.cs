using System;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models.Goal;
using Sportik.Desktop.Infrastructure.Persistence.Entities;

namespace Sportik.Desktop.Infrastructure.Persistence.Mappers
{
    internal sealed class ExerciseGoalMapper
    {
        public static ExerciseGoal ToDomain(UserExerciseGoal userExerciseGoal, int completedRepetitions, bool isExerciseEnabled)
        {
            return new ExerciseGoal(
                ExerciseMapper.ToDomain(userExerciseGoal.Exercise, isExerciseEnabled),
                userExerciseGoal.TargetRepetitions,
                completedRepetitions);
        }

        public static UserExerciseGoal ToEntity(AddExerciseGoalModel addModel, UserExercise exercise)
        {
            return new UserExerciseGoal(
                Guid.NewGuid(),
                addModel.Repetitions,
                CalendarHelper.GetFirstDayOfWeek(addModel.DayInWeek),
                exercise);
        }
    }
}