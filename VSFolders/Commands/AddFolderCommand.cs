// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddFolderCommand.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   AddFolderCommand.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Commands
{
    using System.IO;
    using System.Windows.Threading;
    using Models;
    using Services;

    public class AddFolderCommand : CommandBase
    {
        /// <summary>
        /// Executes the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public override void Execute(object parameter)
        {
            string path = parameter.CheckAs<string>();
           
            var ds = Factory.Resolve<FolderService>();

            if (Directory.Exists(path) && !ds.Files.ContainsKey(path))
            {
                var settings = Factory.Resolve<Settings>();
                if (!settings.OpenFolders.Contains(path))
                {
                    settings.OpenFolders.Add(path);
                }

                Dispatcher.CurrentDispatcher.Invoke(
                    () => ds.Tree.AddLocal(new FileData(null, path)));
            }
        }
    }
}