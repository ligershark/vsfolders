using System.Windows.Forms;
using Microsoft.VSFolders.FastTree;
using Microsoft.VSFolders.Models;
using Microsoft.VSFolders.ViewModels;

namespace Microsoft.VSFolders.Commands
{
    public partial class RemoveCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            var obj = parameter as TreeNode<FileData>;
            if (obj.Value.IsDirectory && obj.Value.Parent == null)
            {
                obj.Root.RemoveWhere(x => x.Value == obj.Value);
                obj.Value.Cleanup();
                VSFoldersPackage.Settings.OpenFolders.Remove(obj.Value.FullPath);
            }
        }
    }
}
