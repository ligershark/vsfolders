// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateFileCommand.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   CreateFileCommand.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Commands
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using Models;

    public class CreateFileCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            FileData obj = parameter.CheckAs<FileData>();

            if (!obj.IsDirectory)
            {
                return;
            }

            string fileName;

            if (AddRename.ShowAsDialog("Provide a folder name", null, out fileName) == DialogResult.OK)
            {
                string newPath = Path.Combine(obj.FullPath, fileName);
                if (File.Exists(newPath))
                {
                    MessageBox.Show("Directory already exists.", "Invalid Directory Name", MessageBoxButtons.OK);
                    return;
                }

                try
                {
                    File.CreateText(newPath);
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to create file.", "Operation failed", MessageBoxButtons.OK);
                }
            }
        }
    }
}