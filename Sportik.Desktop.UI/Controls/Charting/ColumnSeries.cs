using System;
using System.Collections;
using Microsoft.UI.Xaml;

namespace Sportik.Desktop.UI.Controls.Charting
{
    public sealed class ColumnSeries : FrameworkElement
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(ColumnSeries), new PropertyMetadata(null, OnItemsSourceChanged));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty IsCompactProperty =
            DependencyProperty.Register(nameof(IsCompact), typeof(bool), typeof(ColumnSeries), new PropertyMetadata(false, OnIsCompactChanged));

        public bool IsCompact
        {
            get => (bool)GetValue(IsCompactProperty);
            set => SetValue(IsCompactProperty, value);
        }

        public string IndependentValuePath { get; set; }

        public string DependentValuePath { get; set; }

        public Style DataPointStyle { get; set; }

        internal event EventHandler Changed;

        private static void OnIsCompactChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColumnSeries)d).Changed?.Invoke(d, EventArgs.Empty);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColumnSeries)d).Changed?.Invoke(d, EventArgs.Empty);
        }
    }
}

