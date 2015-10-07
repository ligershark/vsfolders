using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Microsoft.VSFolders.Models
{
    internal class MenuItemInfo
    {
        public IContextMenuItem Convert()
        {
            var item = new ContextMenuItem
            {
                Name = Name,
                Icon = IconName,
                Children = (Children ?? new List<MenuItemInfo>()).Select(x => x.Convert()).ToList(), Command = Command
            };

            return item;
        }

        private static readonly MenuItemInfo EmptyValue = new MenuItemInfo();
        public MenuItemInfo(string name, Predicate<FileData> predicate, ICommand command, string iconName = null)
        {
            Name = name;
            Predicate = predicate;
            IconName = iconName;
            Command = command;
        }

        private MenuItemInfo()
        {
        }

        public static MenuItemInfo Empty { get { return EmptyValue; } }
        public List<MenuItemInfo> Children { get; set; }

        public ICommand Command { get; private set; }

        public string IconName { get; private set; }

        public string Name { get; private set; }

        public Predicate<FileData> Predicate { get; private set; }
    }
}