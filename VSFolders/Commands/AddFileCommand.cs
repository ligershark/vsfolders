using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.VSFolders.FastTree;
using Microsoft.VSFolders.Models;
using Microsoft.VSFolders.ViewModels;

namespace Microsoft.VSFolders.Commands
{
    public class AddFileCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            var obj = parameter as TreeNode<FileData>;

            if (obj == null || !obj.Value.IsDirectory)
                return;

            string fileName;

            if (AddRename.ShowAsDialog("Provide a file name", null, out fileName) == DialogResult.OK)
            {
                string newPath = Path.Combine(obj.Value.FullPath, fileName);
                if (File.Exists(newPath))
                {
                    MessageBox.Show("File already exists.", "Invalid File Name", MessageBoxButtons.OK);
                    return;
                }

                try
                {
                    File.CreateText(newPath);
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to create file.", "Operation failed", MessageBoxButtons.OK);
                }
            }
        }
    }
}
