using System.Collections.ObjectModel;
using System.Linq;
using Sportik.Core.Models.Statistics;

namespace Sportik.UWP.ViewModels.Statistics
{
    internal sealed class WeekStatisticsViewModel : ViewModel
    {
        private ObservableCollection<DayStatisticsViewModel> _dayStatistics;

        public ObservableCollection<DayStatisticsViewModel> DayStatistics
        {
            get => _dayStatistics;
            set => SetField(ref _dayStatistics, value);
        }

        public WeekStatisticsViewModel(WeekStatistics weekStatistics)
        {
            DayStatistics = new ObservableCollection<DayStatisticsViewModel>(
                               weekStatistics.DayStatistics.Select(statistics => new DayStatisticsViewModel(statistics)));
        }
    }
}
