using System;
using System.Collections.Generic;

namespace Sportik.Desktop.Infrastructure.DTOs.Statistics
{
    internal sealed class ExerciseStatisticsDto
    {
        public Guid ExerciseId { get; }

        public List<SetDto> Sets { get; }

        public ExerciseStatisticsDto(Guid exerciseId, List<SetDto> sets)
        {
            ExerciseId = exerciseId;
            Sets = sets;
        }
    }
}