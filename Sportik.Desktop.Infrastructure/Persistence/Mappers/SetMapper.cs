using System;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Infrastructure.Persistence.Entities;

namespace Sportik.Desktop.Infrastructure.Persistence.Mappers
{
    internal static class SetMapper
    {
        public static UserSet ToEntity(AddExerciseSetModel addModel)
        {
            return new UserSet(
                addModel.Id ?? Guid.NewGuid(),
                addModel.Repetitions,
                addModel.LoggedAt.ToUniversalTime(),
                addModel.ExerciseId);
        }

        public static ExerciseSet ToDomain(UserSet set, TimeSpan offset)
        {
            return new ExerciseSet(
                set.Id,
                set.Repetitions,
                set.LoggedAt.ToOffset(offset),
                set.ExerciseId);
        }
    }
}