// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextMenuConverter.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   ContextMenuConverter.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using ContextMenu;
    using Services;

    public class ContextMenuConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            ContextMenuService service = Factory.Resolve<ContextMenuService>();
            List<IContextMenuItem> items = service.GetMenuItems(value).ToList();
            return items;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}