using System;
using System.Windows.Forms;
using Microsoft.VSFolders.Build;
using Microsoft.VSFolders.FastTree;
using Microsoft.VSFolders.Models;
using Microsoft.VSFolders.ViewModels;

namespace Microsoft.VSFolders.Commands
{
    public class BuildCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            var obj = parameter as TreeNode<FileData>; 

            if (obj.Value.IsDirectory)
            {
                try
                {
                    var project = ProjectCreator.Create(obj.Value.FullPath + @"\");
                    project.Build(new CompileLogger());
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to execute build.\nThis could be due to inappropriate folder structure or some other failure.", "Build Failure", MessageBoxButtons.OK);
                }

            }
        }
    }
}
