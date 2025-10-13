using System;
using Windows.UI.Xaml.Data;

namespace Sportik.Desktop.UI.Converters
{
    internal class TimeSpanToLongHoursMinutesSecondsStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TimeSpan timeSpan)
            {
                int hours = timeSpan.Hours;
                int minutes = timeSpan.Minutes;
                int seconds = timeSpan.Seconds;

                if (hours >= 1)
                {
                    if (minutes >= 1)
                    {
                        return seconds >= 1 ? $"{hours} hr {minutes} min {seconds} sec" : $"{hours} hr {minutes} min";
                    }

                    return seconds >= 1 ? $"{hours} hr {seconds} sec" : $"{hours} hr";
                }
                else
                {
                    if (minutes >= 1)
                    {
                        return seconds >= 1 ? $"{minutes} min {seconds} sec" : $"{minutes} min";
                    }

                    return $"{seconds} sec";
                }
            }

            return "0 sec";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
