// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrowseForFolderCommand.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   BrowseForFolderCommand.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Commands
{
    using System;
    using System.Windows.Forms;

    public class BrowseForFolderCommand : CommandBase
    {
        public BrowseForFolderCommand(Action<string> commitAction)
        {
            if (commitAction == null)
            {
                throw new ArgumentNullException(nameof(commitAction));
            }

            this.CommitAction = commitAction;
        }

        public Action<string> CommitAction { get; set; }

        public override void Execute(object parameter)
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.CommitAction(ofd.SelectedPath);
            }
        }
    }
}