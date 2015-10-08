// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuItemInfo.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   MenuItemInfo.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using ContextMenu;

    internal class MenuItemInfo
    {
        private static readonly MenuItemInfo EmptyValue = new MenuItemInfo();

        public MenuItemInfo(string name, Predicate<FileData> predicate, ICommand command, string iconName = null)
        {
            this.Name = name;
            this.Predicate = predicate;
            this.IconName = iconName;
            this.Command = command;
        }

        private MenuItemInfo()
        {
        }

        public static MenuItemInfo Empty
        {
            get { return MenuItemInfo.EmptyValue; }
        }

        public List<MenuItemInfo> Children { get; set; }

        public ICommand Command { get; }

        public string IconName { get; }

        public string Name { get; }

        public Predicate<FileData> Predicate { get; private set; }

        public IContextMenuItem Convert()
        {
            ContextMenuItem item = new ContextMenuItem
            {
                Name = this.Name, 
                Icon = this.IconName, 
                Children = (this.Children ?? new List<MenuItemInfo>()).Select(x => x.Convert()).ToList(), 
                Command = this.Command
            };

            return item;
        }
    }
}