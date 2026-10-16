using System;
using Windows.UI.Xaml.Data;
using Sportik.Desktop.Core.Common.DataExchange;

namespace Sportik.Desktop.UI.Converters
{
    internal sealed class DataExchangeOptionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DataExchangeOption option)
            {
                return option switch
                {
                    DataExchangeOption.GoogleSheets => "Google Sheets",
                    _ => "Unknown"
                };
            }

            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}