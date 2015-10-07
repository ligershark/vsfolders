using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VSFolders.Console
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    ///
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
    /// usually implemented by the package implementer.
    ///
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its 
    /// implementation of the IVsUIElementPane interface.
    /// </summary>
    [Guid(GuidList.guidToolWindowConsolePersistanceString)]
    public class ConsoleWindow : ToolWindowPane
    {
        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public ConsoleWindow() :
            base(null)
        {
            // Set the window title reading it from the resources.
            Caption = Resources.CommandWindowTitle;
            // Set the image that will appear on the tab of the window frame
            // when docked with an other window
            // The resource ID correspond to the one defined in the resx file
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip being 16x16.
            BitmapResourceID = 302;
            BitmapIndex = 0;

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
            // the object returned by the Content property.
            base.Content = new ConsoleControl();

            VSFoldersPackage.DTE.Events.DTEEvents.OnBeginShutdown += CleanUpProcesses;
        }

        private static void CleanUpProcesses()
        {
            var toolWin = VSFoldersPackage.GetConsoleWindow(false);

            if (toolWin != null)
            {
                ((ConsoleControl)toolWin.Content).Dispose();
            }

            ConsoleControl.CleanupForVisualStudioShutdown();
        }

        protected override void Dispose(bool disposing)
        {
            ((ConsoleControl)Content).Dispose();
            base.Dispose(disposing);
        }
    }
}