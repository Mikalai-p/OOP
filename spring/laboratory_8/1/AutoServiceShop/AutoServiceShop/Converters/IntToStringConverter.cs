using System;
using System.Globalization;
using System.Windows.Data;

namespace AutoServiceShop.Converters
{
    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return 0;
            if (int.TryParse(value.ToString(), out int result))
                return result;
            return 0;
        }
    }
}