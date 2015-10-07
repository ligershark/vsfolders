using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Microsoft.VSFolders.Converters
{
    public class AssetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value as string ?? parameter as string) == null)
                return null;
            return Imaging.CreateBitmapSourceFromHIcon(((Bitmap)Resources.ResourceManager.GetObject((string)(value as string ?? parameter as string), culture)).GetHicon(), Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
