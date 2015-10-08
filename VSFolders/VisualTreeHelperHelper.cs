// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualTreeHelperHelper.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   VisualTreeHelperHelper.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    public static class VisualTreeHelperHelper
    {
        public static T FindParentOfType<T>(FrameworkElement child)
            where T : DependencyObject
        {
            DependencyObject current = child;

            while (!(current is T))
            {
                if (current == null)
                {
                    return null;
                }

                current = VisualTreeHelper.GetParent(current);
            }

            return (T)current;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    T item = child as T;
                    if (item != null)
                    {
                        yield return item;
                    }

                    foreach (T childOfChild in VisualTreeHelperHelper.FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}