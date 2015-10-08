// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BoolToOpacityConverter.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   BoolToOpacityConverter.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (object.Equals(value, false))
            {
                return parameter;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}