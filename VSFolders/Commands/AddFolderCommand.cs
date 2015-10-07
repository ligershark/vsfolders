using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.VSFolders.FastTree;
using Microsoft.VSFolders.Models;
using Microsoft.VSFolders.ViewModels;

namespace Microsoft.VSFolders.Commands
{
    public class AddFolderCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            var obj = parameter as TreeNode<FileData>;

            if (obj == null || !obj.Value.IsDirectory)
                return;

            string fileName;

            if (AddRename.ShowAsDialog("Provide a folder name", null, out fileName) == DialogResult.OK)
            {
                string newPath = Path.Combine(obj.Value.FullPath, fileName);
                if (Directory.Exists(newPath))
                {
                    MessageBox.Show("Directory already exists.", "Invalid Directory Name", MessageBoxButtons.OK);
                    return;
                }
                try
                {
                    Directory.CreateDirectory(newPath);
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to create directory.", "Operation failed", MessageBoxButtons.OK);
                }
            }
        }
    }
}
