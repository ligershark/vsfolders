// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenInPreviewCommand.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   OpenInPreviewCommand.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Commands
{
    using System.IO;
    using System.Windows;

    /// <summary>
    /// Opens a file in the IDE
    /// </summary>
    public class OpenFileCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            string path = parameter.CheckAs<string>();

            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            try
            {
                if (File.Exists(path) && new FileInfo(path).Length > 5 * 1024 * 1024)
                {
                    if (
                        MessageBox.Show(
                            "The file " + path +
                            " is pretty big and might take quite a while to open.\r\nAre you sure you want to open it?",
                            "Question",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question,
                            MessageBoxResult.No) != MessageBoxResult.Yes)
                    {
                        return;
                    }
                }

                VSFoldersPackage.DTE.ItemOperations.OpenFile(path);
            }
            catch
            {
                // we swallow open failures
            }
        }
    }
}