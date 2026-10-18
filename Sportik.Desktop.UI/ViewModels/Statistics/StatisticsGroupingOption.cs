using Sportik.Desktop.UI.Models.Statistics;

namespace Sportik.Desktop.UI.ViewModels.Statistics
{
    internal sealed class StatisticsGroupingOption
    {
        public string Name { get; }

        public StatisticsGrouping Grouping { get; }

        public StatisticsGroupingOption(string name, StatisticsGrouping grouping)
        {
            Name = name;
            Grouping = grouping;
        }
    }
}
