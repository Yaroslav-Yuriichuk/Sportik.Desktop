using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Sportik.Desktop.UI.Converters
{
    internal sealed class EnumFlagsToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not Enum flagsValue)
            {
                return Visibility.Collapsed;
            }

            if (parameter is not string flagNames)
            {
                return Visibility.Collapsed;
            }

            foreach (string flagName in flagNames.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                if (Enum.TryParse(value.GetType(), flagName.Trim(), out object flagValue) && flagsValue.HasFlag((Enum)flagValue))
                {
                    return Visibility.Visible;
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
