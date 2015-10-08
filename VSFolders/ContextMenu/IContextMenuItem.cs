// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContextMenuItem.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   IContextMenuItem.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.ContextMenu
{
    using System.Collections.Generic;
    using System.Windows.Input;

    public interface IContextMenuItem
    {
        ICommand Command { get; set; }

        string Name { get; set; }

        string Icon { get; set; }

        List<IContextMenuItem> Children { get; set; }
    }
}