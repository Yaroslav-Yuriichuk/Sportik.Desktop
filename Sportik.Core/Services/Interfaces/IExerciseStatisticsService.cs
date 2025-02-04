using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Core.Models.Statistics;

namespace Sportik.Core.Services.Interfaces
{
    public interface IExerciseStatisticsService
    {
        Task<IEnumerable<WeekStatistics>> GetWeekStatisticsAsync(CancellationToken cancellationToken = default);

        DayStatistics AddExerciseStatisticsDelta(ExerciseStatisticsDelta exerciseStatisticsDelta, DateTime date);

        Task<DayStatistics> AddExerciseStatisticsDeltaAsync(ExerciseStatisticsDelta exerciseStatisticsDelta, DateTime date, CancellationToken cancellationToken = default);
    }
}
