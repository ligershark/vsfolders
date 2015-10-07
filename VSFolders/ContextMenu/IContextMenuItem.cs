using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.VSFolders
{
    public interface IContextMenuItem
    {
        ICommand Command { get; set; }
        string Name { get; set; }

        string Icon { get; set; }

        List<IContextMenuItem> Children { get; set; } 
    }
}