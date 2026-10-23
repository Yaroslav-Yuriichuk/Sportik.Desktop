using System;

namespace Sportik.Desktop.Core.Common.DataExchange
{
    [Flags]
    public enum DataExchangeScope
    {
        None = 0,
        Exercises = 1,
        Sets = 2,
        ExercisesAndSets = Exercises | Sets,
    }
}
