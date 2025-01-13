using System;
using Windows.UI.Xaml.Data;

namespace Sportik.Converters
{
    internal sealed class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TimeSpan timeSpan)
            {
                if (timeSpan.TotalHours >= 1)
                {
                    int hours = (int)timeSpan.TotalHours;
                    int minutes = timeSpan.Minutes;

                    return minutes > 0 ? $"{hours}h {minutes}min" : $"{hours}h";
                }
                else
                {
                    return $"{timeSpan.Minutes}min";
                }
            }

            return "0min";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is TimeSpan timeSpan)
            {
                return timeSpan;
            }

            if (value is string timeString)
            {
                try
                {
                    int hours = 0, minutes = 0;

                    if (timeString.Contains("h"))
                    {
                        var hourPart = timeString.Split('h')[0];
                        hours = int.Parse(hourPart.Trim());
                        timeString = timeString.Substring(timeString.IndexOf('h') + 1).Trim();
                    }

                    if (timeString.Contains("min"))
                    {
                        var minutePart = timeString.Replace("min", "").Trim();
                        minutes = int.Parse(minutePart);
                    }

                    return new TimeSpan(hours, minutes, 0);
                }
                catch
                {
                    return TimeSpan.Zero;
                }
            }

            return TimeSpan.Zero;
        }
    }
}
