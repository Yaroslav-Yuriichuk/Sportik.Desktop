using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Desktop.Core.Helpers;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Services.Implementations
{
    internal sealed class ExerciseStatisticsService : IExerciseStatisticsService
    {
        private readonly IExerciseStatisticsRepository _exerciseStatisticsRepository;
        private readonly IDayStatisticsRepository _dayStatisticsRepository;

        public ExerciseStatisticsService(IExerciseStatisticsRepository exerciseStatisticsRepository, IDayStatisticsRepository dayStatisticsRepository)
        {
            _exerciseStatisticsRepository = exerciseStatisticsRepository;
            _dayStatisticsRepository = dayStatisticsRepository;
        }

        public async Task<IEnumerable<WeekStatistics>> GetWeekStatisticsAsync(WeekStatisticsOrder order, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<DayStatistics> AddExerciseStatisticsDeltaAsync(ExerciseStatisticsDelta exerciseStatisticsDelta, DateTime date,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
