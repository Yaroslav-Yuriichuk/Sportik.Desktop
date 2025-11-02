using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Backend.Domain.Common;
using Sportik.Desktop.Core.Models;
using Sportik.Desktop.Core.Models.Statistics;
using Sportik.Desktop.Core.Repositories.Interfaces;
using Sportik.Desktop.Core.Services.Interfaces;

namespace Sportik.Desktop.Core.Services.Implementations
{
    internal sealed class ExerciseStatisticsService : IExerciseStatisticsService
    {
        private readonly IExerciseStatisticsRepository _exerciseStatisticsRepository;

        public ExerciseStatisticsService(IExerciseStatisticsRepository exerciseStatisticsRepository)
        {
            _exerciseStatisticsRepository = exerciseStatisticsRepository;
        }

        public async Task<OperationResult<IEnumerable<WeekStatistics>>> GetWeeklyAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<WeekStatistics> weekStatistics = await _exerciseStatisticsRepository.GetWeeklyAsync(cancellationToken);
                return OperationResult<IEnumerable<WeekStatistics>>.Success(weekStatistics);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult<IEnumerable<WeekStatistics>>.Failure(new[] { "Failed to retrieve weekly exercise statistics." });
            }
        }

        public async Task<OperationResult<ExerciseSet>> AddSetAsync(ExerciseSet set, Guid exerciseId, CancellationToken cancellationToken = default)
        {
            try
            {
                ExerciseSet addedSet = await _exerciseStatisticsRepository.AddSetAsync(set, exerciseId, cancellationToken);
                return OperationResult<ExerciseSet>.Success(addedSet);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return OperationResult<ExerciseSet>.Failure(new[] { "Failed to add the exercise set." });
            }
        }
    }
}
