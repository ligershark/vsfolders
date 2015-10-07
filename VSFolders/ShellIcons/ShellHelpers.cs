using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VSFolders.ShellIcons
{
    static internal class ShellHelpers
    {
        public static void OpenFileInPreviewTab(string file)
        {
            IVsNewDocumentStateContext newDocumentStateContext = null;

            try
            {
                IVsUIShellOpenDocument3 openDoc3 = Package.GetGlobalService(typeof(SVsUIShellOpenDocument)) as IVsUIShellOpenDocument3;

                Guid reason = VSConstants.NewDocumentStateReason.Navigation;
                newDocumentStateContext = openDoc3.SetNewDocumentState((uint)__VSNEWDOCUMENTSTATE.NDS_Provisional, ref reason);

                VSFoldersPackage.DTE.ItemOperations.OpenFile(file);
            }
            finally
            {
                if (newDocumentStateContext != null)
                    newDocumentStateContext.Restore();
            }
        }
    }
}