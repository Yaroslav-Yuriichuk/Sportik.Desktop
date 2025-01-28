using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.UWP.Models.Statistics;

namespace Sportik.UWP.Services.Statistics
{
    internal interface IExerciseStatisticsService
    {
        Task<IEnumerable<WeekStatistics>> GetWeekStatisticsAsync(CancellationToken cancellationToken = default);

        DayStatistics AddExerciseStatisticsDelta(ExerciseStatisticsDelta exerciseStatisticsDelta, DateTime date);
        
        Task<DayStatistics> AddExerciseStatisticsDeltaAsync(ExerciseStatisticsDelta exerciseStatisticsDelta, DateTime date, CancellationToken cancellationToken = default);
    }
}
