using System;

namespace Microsoft.VSFolders.Models
{
    public class FolderChangedEventHandlerArgs : EventArgs
    {
        public string NewPath { get; private set; }

        public FolderChangedEventHandlerArgs(string newPath)
        {
            NewPath = newPath;
        }
    }
}