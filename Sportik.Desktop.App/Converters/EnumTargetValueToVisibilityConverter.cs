using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Sportik.Desktop.App.Converters
{
    internal sealed class EnumTargetValueToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string targetStateString &&
                Enum.TryParse(value.GetType(), targetStateString, out object targetState))
            {
                return value.Equals(targetState) ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
