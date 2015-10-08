// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoveFolderCommand.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   RemoveFolderCommand.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Commands
{
    using System;
    using System.Windows.Threading;
    using Models;
    using Services;

    public class RemoveFolderCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            FileData obj = parameter.CheckAs<FileData>();

            if (obj.IsDirectory && obj.Parent == null)
            {
                obj.Cleanup();
                Factory.Resolve<Settings>().OpenFolders.Remove(obj.FullPath);
                Dispatcher.CurrentDispatcher.BeginInvoke(
                    (Action)(() => Factory.Resolve<FolderService>().Tree.RemoveLocal(obj)));
            }
        }
    }
}