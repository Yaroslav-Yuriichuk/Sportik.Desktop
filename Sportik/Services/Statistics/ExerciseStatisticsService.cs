using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sportik.Helpers;
using Sportik.Models.Statistics;
using Sportik.Services.Events;

namespace Sportik.Services.Statistics
{
    internal sealed class ExerciseStatisticsService : IExerciseStatisticsService
    {
        private readonly IExerciseStatisticsRepository _exerciseStatisticsRepository;
        private readonly IDayStatisticsRepository _dayStatisticsRepository;
        private readonly IEventsService _eventsService;

        public ExerciseStatisticsService(IExerciseStatisticsRepository exerciseStatisticsRepository, IDayStatisticsRepository dayStatisticsRepository,
            IEventsService eventsService)
        {
            _exerciseStatisticsRepository = exerciseStatisticsRepository;
            _dayStatisticsRepository = dayStatisticsRepository;
            _eventsService = eventsService;
        }

        public async Task<IEnumerable<WeekStatistics>> GetWeekStatisticsAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<DayStatistics> dayStatistics = await _dayStatisticsRepository.GetAllAsync(cancellationToken);

            return dayStatistics
                .OrderBy(statistics => statistics.Date)
                .GroupBy(statistics => CalendarHelper.WeekOfYear(statistics.Date))
                .Select(group => new WeekStatistics()
                {
                    DayStatistics = group.ToList(),
                });
        }

        public DayStatistics AddExerciseStatisticsDelta(ExerciseStatisticsDelta exerciseStatisticsDelta, DateTime date)
        {
            date = date.Date;

            DayStatistics dayStatistics = _dayStatisticsRepository.GetByDate(date);

            if (dayStatistics == null)
            {
                dayStatistics = _dayStatisticsRepository.Add(dayStatistics = new DayStatistics { Date = date, });
            }

            ExerciseStatistics exerciseStatistics = dayStatistics.ExerciseStatistics.FirstOrDefault(statistics => statistics.ExerciseId == exerciseStatisticsDelta.Exercise.Id);

            if (exerciseStatistics == null)
            {
                exerciseStatistics = new ExerciseStatistics
                {
                    ExerciseId = exerciseStatisticsDelta.Exercise.Id,
                    DayStatisticsId = dayStatistics.Id,
                    Sets = exerciseStatisticsDelta.Sets,
                    Repetitions = exerciseStatisticsDelta.Repetitions,
                };

                exerciseStatistics = _exerciseStatisticsRepository.Add(exerciseStatistics);
            }
            else
            {
                exerciseStatistics.Sets += exerciseStatisticsDelta.Sets;
                exerciseStatistics.Repetitions += exerciseStatisticsDelta.Repetitions;

                exerciseStatistics = _exerciseStatisticsRepository.Update(exerciseStatistics);
            }

            _eventsService.RaiseEvent(new ExerciseStatisticsDeltaAddedEventArgs(exerciseStatisticsDelta.Exercise, exerciseStatisticsDelta.Sets, exerciseStatisticsDelta.Repetitions));

            return _dayStatisticsRepository.GetById(dayStatistics.Id);
        }

        public async Task<DayStatistics> AddExerciseStatisticsDeltaAsync(ExerciseStatisticsDelta exerciseStatisticsDelta, DateTime date,
            CancellationToken cancellationToken = default)
        {
            date = date.Date;

            DayStatistics dayStatistics = await _dayStatisticsRepository.GetByDateAsync(date, cancellationToken);

            if (dayStatistics == null)
            {
                dayStatistics = await _dayStatisticsRepository.AddAsync(dayStatistics = new DayStatistics { Date = date, }, cancellationToken);
            }

            ExerciseStatistics exerciseStatistics = dayStatistics.ExerciseStatistics.FirstOrDefault(statistics => statistics.ExerciseId == exerciseStatisticsDelta.Exercise.Id);

            if (exerciseStatistics == null)
            {
                exerciseStatistics = new ExerciseStatistics
                {
                    ExerciseId = exerciseStatisticsDelta.Exercise.Id,
                    DayStatisticsId = dayStatistics.Id,
                    Sets = exerciseStatisticsDelta.Sets,
                    Repetitions = exerciseStatisticsDelta.Repetitions,
                };

                exerciseStatistics = await _exerciseStatisticsRepository.AddAsync(exerciseStatistics, cancellationToken);
            }
            else
            {
                exerciseStatistics.Sets += exerciseStatisticsDelta.Sets;
                exerciseStatistics.Repetitions += exerciseStatisticsDelta.Repetitions;

                exerciseStatistics = await _exerciseStatisticsRepository.UpdateAsync(exerciseStatistics, cancellationToken);
            }

            _eventsService.RaiseEvent(new ExerciseStatisticsDeltaAddedEventArgs(exerciseStatisticsDelta.Exercise, exerciseStatisticsDelta.Sets, exerciseStatisticsDelta.Repetitions));
            
            return await _dayStatisticsRepository.GetByIdAsync(dayStatistics.Id, cancellationToken);
        }
    }
}
