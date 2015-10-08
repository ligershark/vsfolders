// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShellHelpers.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   ShellHelpers.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.ShellIcons
{
    using System;
    using VisualStudio;
    using VisualStudio.Shell;
    using VisualStudio.Shell.Interop;

    internal static class ShellHelpers
    {
        public static void OpenFileInPreviewTab(string file)
        {
            IVsNewDocumentStateContext newDocumentStateContext = null;

            try
            {
                IVsUIShellOpenDocument3 openDoc3 =
                    Package.GetGlobalService(typeof (SVsUIShellOpenDocument)) as IVsUIShellOpenDocument3;

                Guid reason = VSConstants.NewDocumentStateReason.Navigation;
                newDocumentStateContext = openDoc3.SetNewDocumentState(
                    (uint)__VSNEWDOCUMENTSTATE.NDS_Provisional,
                    ref reason);

                VSFoldersPackage.DTE.ItemOperations.OpenFile(file);
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                if (newDocumentStateContext != null)
                {
                    newDocumentStateContext.Restore();
                }
            }
        }
    }
}