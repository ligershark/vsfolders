using System.Windows.Forms;
using Microsoft.VSFolders.FastTree;
using Microsoft.VSFolders.Models;
using Microsoft.VSFolders.ViewModels;

namespace Microsoft.VSFolders.Commands
{
    public class OpenProjectCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            var obj = parameter as TreeNode<FileData>; ;
            if (VSFoldersPackage.DTE.Solution.IsOpen)
            {
                VSFoldersPackage.DTE.Solution.Close();
            }

            VSFoldersPackage.DTE.Solution.AddFromFile(obj.Value.FullPath);
        }
    }
}
