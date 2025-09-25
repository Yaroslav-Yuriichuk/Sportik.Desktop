using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace Sportik.Desktop.App.Converters
{
    internal sealed class DateTimeOffsetToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTimeOffset dateTimeOffset)
            {
                if (parameter is string format)
                {
                    return string.Format($"{{0:{format}}}", dateTimeOffset);
                }

                return dateTimeOffset.ToString(new CultureInfo("en-US"));
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
