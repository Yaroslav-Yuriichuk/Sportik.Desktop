using System.Collections.Generic;

namespace Sportik.Desktop.UI.ViewModels.Dashboard
{
    internal sealed class DashboardGroupingOption
    {
        public string Name { get; }

        public DashboardGrouping Grouping { get; }

        public IEnumerable<DashboardTargetOption> TargetOptions { get; }

        public DashboardGroupingOption(string name, DashboardGrouping grouping, IEnumerable<DashboardTargetOption> targetOptions)
        {
            Name = name;
            Grouping = grouping;
            TargetOptions = targetOptions;
        }
    }
}