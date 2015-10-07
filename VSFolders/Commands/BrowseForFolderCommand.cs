using System;
using System.Windows.Forms;

namespace Microsoft.VSFolders.Commands
{
    public class BrowseForFolderCommand : CommandBase
    {
        public Action<string> CommitAction { get; set; }

        public BrowseForFolderCommand(Action<string> commitAction)
        {
            CommitAction = commitAction;
        }

        public override void Execute(object parameter)
        {
            var ofd = new FolderBrowserDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                CommitAction(ofd.SelectedPath);
            }
        }
    }
}
