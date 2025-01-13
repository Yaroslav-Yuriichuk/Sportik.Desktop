using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Models.Notifications;
using Sportik.Models.Statistics;
using Sportik.Services.Notifications;
using Sportik.Services.Statistics;

namespace Sportik.ViewModels.Statistics
{
    internal sealed class StatisticsViewModel : ViewModel, IDisposable
    {
        private ObservableCollection<WeekStatisticsViewModel> _weekStatistics;

        public ObservableCollection<WeekStatisticsViewModel> WeekStatistics
        {
            get => _weekStatistics;
            set => SetField(ref _weekStatistics, value);
        }

        private IExerciseStatisticsService ExerciseStatisticsService => App.ServiceProvider.GetService<IExerciseStatisticsService>();
        private INotificationService NotificationService => App.ServiceProvider.GetService<INotificationService>();

        private readonly CancellationTokenSource _loadCts = new CancellationTokenSource();

        public StatisticsViewModel()
        {
            /*IEnumerable<WeekStatistics> weekStatistics = new WeekStatistics[]
            {
                new WeekStatistics
                {
                    DayStatistics = new List<DayStatistics>
                    {
                        new DayStatistics
                        {
                            Date = DateTime.Today,
                            ExerciseStatistics = new List<ExerciseStatistics>
                            {
                                new ExerciseStatistics
                                {
                                    Exercise = new Exercise { Name = "Push ups" },
                                    Sets = 3,
                                    Repetitions = 10
                                },
                            }
                        },
                        new DayStatistics
                        {
                            Date = DateTime.Today.AddDays(-1),
                            ExerciseStatistics = new List<ExerciseStatistics>
                            {
                                new ExerciseStatistics
                                {
                                    Exercise = new Exercise { Name = "Pull ups" },
                                    Sets = 3,
                                    Repetitions = 20
                                },
                                new ExerciseStatistics
                                {
                                    Exercise = new Exercise { Name = "Push ups" },
                                    Sets = 3,
                                    Repetitions = 10
                                },
                            }
                        }
                    }
                }
            };

            WeekStatistics = new ObservableCollection<WeekStatisticsViewModel>(
                weekStatistics.Select(statistics => new WeekStatisticsViewModel(statistics)));*/

            _ = LoadDayStatisticsAsync(_loadCts.Token);

            NotificationService.ShowReminder(new ReminderNotification()
            {
                Title = "Reminder",
            });
        }

        public void Dispose()
        {
            _loadCts.Cancel();
        }

        private async Task LoadDayStatisticsAsync(CancellationToken cancellationToken)
        {
            IEnumerable<WeekStatistics> weekStatistics = await ExerciseStatisticsService.GetWeekStatisticsAsync(cancellationToken);

            WeekStatistics = new ObservableCollection<WeekStatisticsViewModel>(
                               weekStatistics.Select(statistics => new WeekStatisticsViewModel(statistics)));
        }
    }
}
