namespace Sportik.Desktop.UI.Models.Dashboard
{
    internal sealed class StatisticsCard
    {
        public string Title { get; }

        public string Value { get; }

        public string Subtitle { get; }

        public StatisticsCard(string title, string value, string subtitle)
        {
            Title = title;
            Value = value;
            Subtitle = subtitle;
        }
    }
}
