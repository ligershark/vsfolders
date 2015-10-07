using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace Microsoft.VSFolders.Converters
{
    public class WhyIsThisNecessaryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = (value as IContextMenuItem ?? parameter as IContextMenuItem);
            if (item == null)
                return null;
            var grid = new Grid();
            
            if (item.Icon != null)
            {
                var source = Imaging.CreateBitmapSourceFromHIcon(((Bitmap) Resources.ResourceManager.GetObject(item.Icon, culture)).GetHicon(), Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                grid.Children.Add(new Image { Source = source });
            }

            ServiceContainer.Resolve<ContextMenuService>().ContextMenuFrameworkElement = () => VisualTreeHelperHelper.FindParentOfType<MenuItem>(grid);
            return grid;
        }



        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
