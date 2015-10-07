using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.VSFolders
{
    public class ContextMenuItem : IContextMenuItem
    {

        public ContextMenuItem()
        {
            Children = new List<IContextMenuItem>();
        }
        public ICommand Command { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public List<IContextMenuItem> Children { get; set; }
    }
}