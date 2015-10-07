using System.Windows.Forms;
using Microsoft.VSFolders.FastTree;
using Microsoft.VSFolders.Models;
using Microsoft.VSFolders.ViewModels;

namespace Microsoft.VSFolders.Commands
{
    public class RenameCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            var obj = parameter as TreeNode<FileData>; ;
            if (obj == null)
                return;

            string fileName;

            if (AddRename.ShowAsDialog("Provide a new name", obj.Value.Name, out fileName) == DialogResult.OK)
            {
                if (fileName == obj.Value.Name)
                    return;
                obj.Value.TempName = fileName;
                obj.Value.CommitRename();
            }
        }
    }
}
