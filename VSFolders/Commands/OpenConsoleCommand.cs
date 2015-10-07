using Microsoft.VSFolders.Console;
using Microsoft.VSFolders.FastTree;
using Microsoft.VSFolders.Models;
using Microsoft.VSFolders.ViewModels;

namespace Microsoft.VSFolders.Commands
{
    public class OpenConsoleCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            var obj = parameter as TreeNode<FileData>; ;

            var console = VSFoldersPackage.GetConsoleWindow(true);
            ((ConsoleControl)console.Content).OpenAt(obj.Value);
        }
    }
}
