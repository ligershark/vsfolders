// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrowseCommand.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   BrowseCommand.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Commands
{
    using System.Diagnostics;
    using System.IO;
    using Models;

    public class BrowseCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            FileData obj = parameter.CheckAs<FileData>();

            string path = obj.IsDirectory ? obj.FullPath : Path.GetDirectoryName(obj.FullPath);

            Process.Start("explorer.exe", path);
        }
    }
}