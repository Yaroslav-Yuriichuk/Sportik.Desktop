using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace Sportik.UWP.Converters
{
    internal sealed class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTime)
            {
                if (parameter is string format)
                {
                    return string.Format($"{{0:{format}}}", dateTime);
                }

                return dateTime.ToString(new CultureInfo("en-US"));
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
