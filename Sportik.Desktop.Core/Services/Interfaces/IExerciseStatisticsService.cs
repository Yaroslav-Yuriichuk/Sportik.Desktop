using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Models.Statistics;

namespace Sportik.Desktop.Core.Services.Interfaces
{
    public interface IExerciseStatisticsService
    {
        Task<IEnumerable<WeekStatistics>> GetWeekStatisticsAsync(WeekStatisticsOrder order, CancellationToken cancellationToken = default);

        Task<DayStatistics> AddExerciseStatisticsDeltaAsync(ExerciseStatisticsDelta exerciseStatisticsDelta, DateTime date, CancellationToken cancellationToken = default);
    }
}
