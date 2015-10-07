using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Microsoft.VSFolders.Converters
{
    public class ContextMenuConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            var service = ServiceContainer.Resolve<ContextMenuService>();
            var items = service.GetMenuItems(value).ToList();
            return items;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
