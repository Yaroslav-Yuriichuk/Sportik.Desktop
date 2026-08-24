namespace Sportik.Desktop.UI.ViewModels.Dashboard
{
    internal sealed class DashboardGroupingOption
    {
        public string Name { get; }

        public DashboardGrouping Grouping { get; }

        public DashboardGroupingOption(string name, DashboardGrouping grouping)
        {
            Name = name;
            Grouping = grouping;
        }
    }
}