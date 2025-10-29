using System;
using Windows.UI.Xaml.Data;

namespace Sportik.Desktop.UI.Converters
{
    public sealed class GuidToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is Guid guid
                ? guid.ToString(parameter as string ?? "D")
                : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Guid.TryParse(value as string, out Guid guid) ? guid : Guid.Empty;
        }
    }
}