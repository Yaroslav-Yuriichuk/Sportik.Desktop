using System;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models.Goal;
using Sportik.Desktop.Infrastructure.Persistence.Entities;

namespace Sportik.Desktop.Infrastructure.Persistence.Mappers
{
    internal sealed class ExerciseGoalMapper
    {
        public static ExerciseGoal ToDomain(UserExerciseGoal userExerciseGoal, bool isEnabled)
        {
            return new ExerciseGoal(
                ExerciseMapper.ToDomain(userExerciseGoal.Exercise, isEnabled),
                userExerciseGoal.Repetitions);
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