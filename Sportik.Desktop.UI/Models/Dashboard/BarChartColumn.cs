namespace Sportik.Desktop.UI.Models.Dashboard
{
    internal sealed class BarChartColumn
    {
        public object Key { get; }

        public object Value { get; }

        public BarChartColumn(object key, object value)
        {
            Key = key;
            Value = value;
        }
    }
}