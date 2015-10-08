// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextMenuItem.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   ContextMenuItem.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.ContextMenu
{
    using System.Collections.Generic;
    using System.Windows.Input;

    public class ContextMenuItem : IContextMenuItem
    {
        public ContextMenuItem()
        {
            this.Children = new List<IContextMenuItem>();
        }

        public ICommand Command { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public List<IContextMenuItem> Children { get; set; }
    }
}