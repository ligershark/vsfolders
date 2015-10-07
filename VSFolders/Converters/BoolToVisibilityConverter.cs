using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.VSFolders.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (!Equals(parameter, null) ^ Equals(value, true)) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
