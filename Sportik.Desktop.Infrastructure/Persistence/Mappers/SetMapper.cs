using System;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Infrastructure.Persistence.Entities;

namespace Sportik.Desktop.Infrastructure.Persistence.Mappers
{
    internal static class SetMapper
    {
        public static UserSet ToEntity(AddExerciseSetModel addModel)
        {
            DateTimeOffset loggedAt = DateTimeOffset.FromUnixTimeMilliseconds(addModel.LoggedAt.ToUnixTimeMilliseconds());

            return new UserSet(
                addModel.Id ?? Guid.NewGuid(),
                addModel.Repetitions,
                loggedAt,
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