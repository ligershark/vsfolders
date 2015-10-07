using System.Windows;
using System.Windows.Media;

namespace Microsoft.VSFolders
{
    public static class VisualTreeHelperHelper
    {
        public static T FindParentOfType<T>(FrameworkElement child)
            where T : DependencyObject
        {
            DependencyObject current = child;

            while (current as T == null)
            {
                if (current == null)
                {
                    return null;
                }

                current = VisualTreeHelper.GetParent(current);
            }

            return (T) current;
        }
    }
}
