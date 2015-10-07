using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.VSFolders.FastTree;
using Microsoft.VSFolders.Models;
using Microsoft.VSFolders.ViewModels;
using Microsoft.VSFolders.WindowsShell;
using Point = System.Drawing.Point;

namespace Microsoft.VSFolders.Commands
{
    public class ShowShellMenuCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            var obj = parameter as TreeNode<FileData>;
            using (var menu = new ShellContextMenu())
            {
                var target = (FrameworkElement)VSFoldersPackage.DemandFoldersWindow().Content;
                System.Windows.Point pt;
                var ctx = VisualTreeHelperHelper.FindParentOfType<System.Windows.Controls.ContextMenu>(ServiceContainer.Resolve<ContextMenuService>().ContextMenuFrameworkElement());

                if (ctx != null)
                {
                    pt = ctx.PointToScreen(new System.Windows.Point());
                }
                else
                {
                    pt = Mouse.GetPosition(target);
                    pt = target.PointToScreen(pt);
                }

                var fsi = obj.Value.IsDirectory ? (FileSystemInfo)new DirectoryInfo(obj.Value.FullPath) : new FileInfo(obj.Value.FullPath);
                menu.ShowContextMenu(new[] { fsi }, new Point((int)pt.X, (int)pt.Y));
            }
        }
    }
}
