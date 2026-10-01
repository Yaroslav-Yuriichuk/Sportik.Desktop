using System;
using Windows.UI.Xaml.Data;

namespace Sportik.Desktop.UI.Converters
{
    internal sealed class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int i)
            {
                return i.ToString();
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string s && int.TryParse(s, out int result))
            {
                return result;
            }

            return 0;
        }
    }
}