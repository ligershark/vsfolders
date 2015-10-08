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
    using Services;

    public class NavigateToCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            Factory.Resolve<FolderNavigationService>().NavigateTo(parameter as string);
        }
    }
}