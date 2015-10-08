// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenProjectCommand.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   OpenProjectCommand.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Commands
{
    public class SyncActiveDocumentCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            if (VSFoldersPackage.DTE.ActiveDocument == null)
                return;

            string path = VSFoldersPackage.DTE.ActiveDocument.FullName;
            new NavigateToCommand().Execute(path);
        }
    }
}