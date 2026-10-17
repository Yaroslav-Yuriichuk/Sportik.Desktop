using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Sportik.Desktop.UI.Controls.Charting
{
    public class Chart : ContentControl
    {
        private const string PlotPartName = "PART_PlotGrid";
        private const string LabelsPartName = "PART_LabelsGrid";

        private Grid _plotGrid;
        private Grid _labelsGrid;

        public Chart()
        {
            DataContextChanged += (sender, args) => PushDataContext();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _plotGrid = GetTemplateChild(PlotPartName) as Grid;
            _labelsGrid = GetTemplateChild(LabelsPartName) as Grid;

            PushDataContext();
            Rebuild();
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            if (oldContent is ColumnSeries oldSeries)
            {
                oldSeries.Changed -= Series_Changed;
            }

            if (newContent is ColumnSeries newSeries)
            {
                newSeries.Changed += Series_Changed;
            }

            PushDataContext();
            Rebuild();
        }

        private void PushDataContext()
        {
            if (Content is ColumnSeries series)
            {
                series.DataContext = DataContext;
            }
        }

        private void Series_Changed(object sender, EventArgs e)
        {
            Rebuild();
        }

        private void Rebuild()
        {
            if (_plotGrid == null || _labelsGrid == null)
            {
                return;
            }

            _plotGrid.ColumnDefinitions.Clear();
            _plotGrid.Children.Clear();
            _labelsGrid.ColumnDefinitions.Clear();
            _labelsGrid.Children.Clear();

            if (!(Content is ColumnSeries columnSeries) || !(columnSeries.ItemsSource is IEnumerable itemsEnumerable))
            {
                return;
            }

            List<object> items = itemsEnumerable.Cast<object>().ToList();

            if (items.Count == 0)
            {
                return;
            }

            double maximum = 0;
            List<double> values = new List<double>(items.Count);

            foreach (object item in items)
            {
                double value = ToDouble(GetPathValue(item, columnSeries.DependentValuePath));

                if (value < 0)
                {
                    value = 0;
                }

                values.Add(value);

                if (value > maximum)
                {
                    maximum = value;
                }
            }

            for (int i = 0; i < items.Count; i++)
            {
                _plotGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                _labelsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            for (int i = 0; i < items.Count; i++)
            {
                object item = items[i];
                double value = values[i];
                double lowerWeight = value;
                double upperWeight = maximum - value;

                if (lowerWeight <= 0 && upperWeight <= 0)
                {
                    upperWeight = 1;
                }

                Grid column = new Grid();
                column.RowDefinitions.Add(new RowDefinition { Height = new GridLength(upperWeight, GridUnitType.Star) });
                column.RowDefinitions.Add(new RowDefinition { Height = new GridLength(lowerWeight, GridUnitType.Star) });

                object independentValue = GetPathValue(item, columnSeries.IndependentValuePath);
                object dependentValue = GetPathValue(item, columnSeries.DependentValuePath);

                ColumnDataPoint dataPoint = new ColumnDataPoint
                {
                    Style = columnSeries.DataPointStyle,
                    FormattedDependentValue = dependentValue?.ToString(),
                    Margin = new Thickness(16, 0, 16, 0),
                };

                dataPoint.Loaded += DataPoint_Loaded;

                Grid.SetRow(dataPoint, 1);
                column.Children.Add(dataPoint);

                Grid.SetColumn(column, i);
                _plotGrid.Children.Add(column);

                TextBlock label = new TextBlock
                {
                    Text = independentValue?.ToString() ?? string.Empty,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(4, 4, 4, 0),
                    TextTrimming = TextTrimming.CharacterEllipsis,
                };

                Grid.SetColumn(label, i);
                _labelsGrid.Children.Add(label);
            }
        }

        private static object GetPathValue(object item, string path)
        {
            if (item == null || string.IsNullOrEmpty(path))
            {
                return item;
            }

            return item.GetType().GetProperty(path)?.GetValue(item);
        }

        private static double ToDouble(object value)
        {
            if (value is IConvertible convertible)
            {
                try
                {
                    return convertible.ToDouble(CultureInfo.CurrentCulture);
                }
                catch (Exception)
                {
                    return 0;
                }
            }

            return 0;
        }

        private static void DataPoint_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Control control)
            {
                VisualStateManager.GoToState(control, "Shown", true);
            }
        }
    }
}
