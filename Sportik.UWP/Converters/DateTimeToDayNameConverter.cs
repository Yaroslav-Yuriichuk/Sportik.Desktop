using System;
using Windows.UI.Xaml.Data;
using Sportik.UWP.Helpers;

namespace Sportik.UWP.Converters
{
    public sealed class DateTimeToDayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTime)
            {
                return CalendarHelper.DayName(dateTime);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
