// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteCommand.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   DeleteCommand.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Commands
{
    using System.IO;
    using System.Windows.Forms;
    using Models;

    public class DeleteCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            FileData obj = parameter.CheckAs<FileData>();
        
            if (obj.IsDirectory &&
                MessageBox.Show(
                    "Are you sure you want to delete this directory?", 
                    "Delete Confirmation", 
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Directory.Delete(obj.FullPath, true);
            }
            else if (
                MessageBox.Show(
                    "Are you sure you want to delete this file?", 
                    "Delete Confirmation", 
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                File.Delete(obj.FullPath);
            }
        }
    }
}