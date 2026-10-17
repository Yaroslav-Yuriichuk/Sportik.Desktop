using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Sportik.Desktop.UI.Controls.Charting
{
    public sealed class ColumnDataPoint : Control
    {
        public static readonly DependencyProperty FormattedDependentValueProperty =
            DependencyProperty.Register(nameof(FormattedDependentValue), typeof(string), typeof(ColumnDataPoint), new PropertyMetadata(null));

        public string FormattedDependentValue
        {
            get => (string)GetValue(FormattedDependentValueProperty);
            set => SetValue(FormattedDependentValueProperty, value);
        }
    }
}

