using System.IO;
using System.Windows.Forms;
using Microsoft.VSFolders.FastTree;
using Microsoft.VSFolders.Models;
using Microsoft.VSFolders.ViewModels;

namespace Microsoft.VSFolders.Commands
{
    public class DeleteCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            var obj = parameter as TreeNode<FileData>;

            if (obj.Value.IsDirectory && MessageBox.Show("Are you sure you want to delete this directory?", "Delete Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Directory.Delete(obj.Value.FullPath, true);
            }
            else if (MessageBox.Show("Are you sure you want to delete this file?", "Delete Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                File.Delete(obj.Value.FullPath);
            }
        }
    }
}
