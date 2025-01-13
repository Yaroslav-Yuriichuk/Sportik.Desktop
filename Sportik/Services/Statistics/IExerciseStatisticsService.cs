using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Models.Statistics;

namespace Sportik.Services.Statistics
{
    internal interface IExerciseStatisticsService
    {
        Task<IEnumerable<WeekStatistics>> GetWeekStatisticsAsync(CancellationToken cancellationToken = default);

        Task<DayStatistics> AddExerciseStatisticsDeltaAsync(ExerciseStatisticsDelta exerciseStatisticsDelta, DateTime date, CancellationToken cancellationToken = default);
    }
}
