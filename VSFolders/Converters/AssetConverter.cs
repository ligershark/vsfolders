// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetConverter.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   AssetConverter.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Converters
{
    using System;
    using System.Collections.Concurrent;
    using System.Drawing;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;

    public class AssetConverter : IValueConverter
    {
        private static readonly ConcurrentDictionary<string, BitmapSource> Cache = new ConcurrentDictionary<string, BitmapSource>(StringComparer.OrdinalIgnoreCase); 
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string param = value as string ?? parameter as string;
            if (string.IsNullOrEmpty(param))
            {
                return null;
            }

            return AssetConverter.Cache.GetOrAdd(
                param,
                x => Imaging.CreateBitmapSourceFromHIcon(
                    ((Bitmap)Resources.ResourceManager.GetObject(x, culture))
                        .GetHicon(),
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}