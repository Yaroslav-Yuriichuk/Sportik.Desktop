using System;
using Windows.UI.Xaml.Data;

namespace Sportik.Desktop.UI.Converters
{
    internal sealed class TimeSpanToLongHoursMinutesStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TimeSpan timeSpan)
            {
                int hours = timeSpan.Hours;
                int minutes = timeSpan.Minutes;

                if (hours >= 1)
                {
                    return minutes >= 1 ? $"{hours} hr {minutes} min" : $"{hours} hr";
                }

                return $"{minutes} min";
            }

            return "0 min";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
