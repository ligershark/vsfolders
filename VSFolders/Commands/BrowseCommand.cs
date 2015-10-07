using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.VSFolders.FastTree;
using Microsoft.VSFolders.Models;
using Microsoft.VSFolders.ViewModels;

namespace Microsoft.VSFolders.Commands
{
    public class BrowseCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            var obj = parameter as TreeNode<FileData>;

            if (obj.Value.IsDirectory)
            {
                Process.Start("explorer.exe", obj.Value.FullPath);
            }
            else
            {
                Process.Start(obj.Value.FullPath);
            }
        }
    }
}
