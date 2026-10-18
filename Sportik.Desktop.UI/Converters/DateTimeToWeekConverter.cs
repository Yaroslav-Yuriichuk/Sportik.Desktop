using System;
using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace Sportik.Desktop.UI.Converters;

internal sealed class DateTimeToWeekConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DateTime dateTime)
        {
            return ISOWeek.GetWeekOfYear(dateTime);
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}