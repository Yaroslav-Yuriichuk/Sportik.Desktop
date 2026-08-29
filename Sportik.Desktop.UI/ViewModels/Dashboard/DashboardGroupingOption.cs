using System.Collections.Generic;
using Sportik.Desktop.UI.Models.Dashboard;

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