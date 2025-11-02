using System.Collections.Generic;
using Sportik.Desktop.Infrastructure.DTOs.Exercises;

namespace Sportik.Desktop.Infrastructure.DTOs.Statistics
{
    internal sealed class ExerciseStatisticsDto
    {
        public ExerciseDto Exercise { get; }

        public List<SetDto> Sets { get; }

        public ExerciseStatisticsDto(ExerciseDto exercise, List<SetDto> sets)
        {
            Exercise = exercise;
            Sets = sets;
        }
    }
}