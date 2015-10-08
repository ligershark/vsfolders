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
    using Models;

    public class OpenProjectCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            FileData obj = parameter.CheckAs<FileData>();

            if (VSFoldersPackage.DTE.Solution.IsOpen)
            {
                VSFoldersPackage.DTE.Solution.Close();
            }

            VSFoldersPackage.DTE.Solution.AddFromFile(obj.FullPath);
        }
    }
}