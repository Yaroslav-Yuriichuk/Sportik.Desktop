using System.Collections.Generic;
using Sportik.Desktop.Core.Models;

namespace Sportik.Desktop.Core.Common.DataExchange
{
    public sealed class DataExchangeContext
    {
        public DataExchangeScope Scope { get; }

        public IEnumerable<Exercise> Exercises { get; }

        public IEnumerable<ExerciseSet> Sets { get; }

        public DataExchangeContext(DataExchangeScope scope, IEnumerable<Exercise> exercises = null, IEnumerable<ExerciseSet> sets = null)
        {
            Scope = scope;
            Exercises = exercises;
            Sets = sets;
        }
    }
}
