using System;
using System.Collections.Generic;

namespace Sportik.Desktop.Core.Models.Statistics
{
    public sealed class ExerciseStatistics
    {
        public Guid ExerciseId { get; }

        public List<ExerciseSet> Sets { get; }

        public ExerciseStatistics(Guid exerciseId, List<ExerciseSet> sets)
        {
            ExerciseId = exerciseId;
            Sets = sets;
        }
    }
}
