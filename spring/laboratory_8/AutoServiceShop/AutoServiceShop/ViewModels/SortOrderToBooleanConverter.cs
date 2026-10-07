using System;
using System.Globalization;
using System.Windows.Data;

namespace AutoServiceShop.ViewModels
{
    public class SortOrderToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SortOrder current && parameter is SortOrder target)
                return current == target;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && isChecked && parameter is SortOrder target)
                return target;
            return Binding.DoNothing;
        }
    }
}