// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenameCommand.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   RenameCommand.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Commands
{
    using System.IO;
    using System.Windows.Forms;
    using Models;

    public class RenameCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            FileData obj = parameter.CheckAs<FileData>();

            string fileName;

            if (AddRename.ShowAsDialog("Provide a new name", obj.Name, out fileName) == DialogResult.OK)
            {
                if (fileName == obj.Name)
                {
                    return;
                }

                if (obj.IsDirectory)
                {
                    Directory.Move(obj.FullPath, Path.Combine(Path.GetDirectoryName(obj.FullPath), fileName));
                }
                else
                {
                    File.Move(obj.FullPath, Path.Combine(Path.GetDirectoryName(obj.FullPath), fileName));
                }
            }
        }
    }
}