using Sportik.Desktop.UI.Models.Dashboard;

namespace Sportik.Desktop.UI.ViewModels.Dashboard
{
    internal sealed class DashboardTargetOption
    {
        public string Name { get; }

        public DashboardTarget Target { get; }

        public DashboardTargetOption(string name, DashboardTarget target)
        {
            Name = name;
            Target = target;
        }
    }
}