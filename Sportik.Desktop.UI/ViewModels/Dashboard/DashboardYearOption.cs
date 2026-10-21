namespace Sportik.Desktop.UI.ViewModels.Dashboard
{
    internal sealed class DashboardYearOption
    {
        public string Name { get; }

        public int Year { get; }

        public DashboardYearOption(string name, int year)
        {
            Name = name;
            Year = year;
        }
    }
}