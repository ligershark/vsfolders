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
    using Models;
    using ShellIcons;

    /// <summary>
    /// The open in preview command.
    /// </summary>
    public class OpenInPreviewCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            string path = parameter.CheckAs<string>();

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (Factory.Resolve<Settings>().PreviewItems)
            {
                try
                {
                    if (File.Exists(path) && new FileInfo(path).Length < 1024 * 1024)
                    {
                        ShellHelpers.OpenFileInPreviewTab(path);
                    }
                }
                catch
                {
                    // nothing to do here if this fails
                }
            }
        }
    }
}