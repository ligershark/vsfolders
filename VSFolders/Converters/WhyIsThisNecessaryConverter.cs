// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WhyIsThisNecessaryConverter.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   WhyIsThisNecessaryConverter.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Converters
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;
    using ContextMenu;
    using Image = System.Windows.Controls.Image;

    /// <summary>
    /// This class is necessary to get menuitems to appear with their appropriate icons.
    /// </summary>
    public class WhyIsThisNecessaryConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IContextMenuItem item = value as IContextMenuItem ?? parameter as IContextMenuItem;
            if (item == null)
            {
                return null;
            }

            Grid grid = new Grid();

            if (item.Icon != null)
            {
                BitmapSource source =
                    Imaging.CreateBitmapSourceFromHIcon(
                        ((Bitmap)Resources.ResourceManager.GetObject(item.Icon, culture)).GetHicon(), 
                        Int32Rect.Empty, 
                        BitmapSizeOptions.FromEmptyOptions());
                grid.Children.Add(new Image {Source = source});
            }
            return grid;
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}