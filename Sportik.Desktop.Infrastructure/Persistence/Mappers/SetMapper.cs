using System;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Infrastructure.Persistence.Entities;

namespace Sportik.Desktop.Infrastructure.Persistence.Mappers
{
    internal static class SetMapper
    {
        public static UserSet ToEntity(ExerciseSet set, Guid exerciseId)
        {
            return new UserSet(
                Guid.NewGuid(),
                set.Repetitions,
                set.LoggedAt,
                exerciseId);
        }

        public static ExerciseSet ToDomain(UserSet set)
        {
            return new ExerciseSet(
                set.Repetitions,
                set.LoggedAt);
        }
    }
}