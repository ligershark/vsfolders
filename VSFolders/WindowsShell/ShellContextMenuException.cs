using System;

namespace Microsoft.VSFolders.WindowsShell
{
    public class ShellContextMenuException : Exception
    {
        public ShellContextMenuException()
        {
        }

        public ShellContextMenuException(string message)
            : base(message)
        {
        }
    }
}