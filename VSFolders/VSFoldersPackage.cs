using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VSFolders.Console;
using Microsoft.VSFolders.Settings;
using Newtonsoft.Json;

namespace Microsoft.VSFolders
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    // This attribute registers a tool window exposed by this package.
    [ProvideToolWindow(typeof(FoldersWindow))]
    [ProvideToolWindow(typeof(ConsoleWindow))]
    [ProvideOptionPage(typeof(GeneralSettingsPage), "VS Folders", "General", 101, 210, true, new[] { "General", "Folders" })]
    [ProvideOptionPage(typeof(FtpSettingsPage), "VS Folders", "FTP Publishing", 101, 211, true, new[] { "FTP", "Publish", "Upload" })]
    [ProvideOptionPage(typeof(ConsoleSettingsPage), "VS Folders", "Console", 101, 212, true, new[] { "Console", "Command Line" })]
    [Guid(GuidList.guidVSFoldersPkgString)]
    public sealed class VSFoldersPackage : Package
    {
        private static VSFoldersPackage _instance;

        private static readonly Lazy<DTE2> DTEValue = new Lazy<DTE2>(() => ServiceProvider.GlobalProvider.GetService(typeof (DTE)) as DTE2);

        public static DTE2 DTE {get { return DTEValue.Value; }}

        private void ShowDialogPageInternal<T>()
            where T : DialogPage
        {
            ShowOptionPage(typeof(T));
        }

        private static readonly ConcurrentDictionary<Type, DialogPage> DialogPageInstanceLookup = new ConcurrentDictionary<Type, DialogPage>();

        private T GetDialogPageInternal<T>()
            where T : DialogPage
        {
            return (T) DialogPageInstanceLookup.GetOrAdd(typeof (T), o => GetDialogPage(typeof (T)));
        }

        public static T GetDialogPage<T>()
            where T : DialogPage
        {
            return _instance.GetDialogPageInternal<T>();
        }


        public static void ShowDialogPage<T>()
            where T : DialogPage
        {
            _instance.ShowDialogPageInternal<T>();
        }

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public VSFoldersPackage()
        {
            _instance = this;
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", ToString()));
        }

        /// <summary>
        /// This function is called when the user clicks the menu item that shows the 
        /// tool window. See the Initialize method to see how the menu item is associated to 
        /// this function using the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void ShowToolFoldersWindow(object sender, EventArgs e)
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            var window = FindToolWindow(typeof(FoldersWindow), 0, true);
            
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }
            
            var windowFrame = (IVsWindowFrame)window.Frame;
            VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        /// <summary>
        /// This function is called when the user clicks the menu item that shows the 
        /// tool window. See the Initialize method to see how the menu item is associated to 
        /// this function using the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void ShowToolConsoleWindow(object sender, EventArgs e)
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            var window = FindToolWindow(typeof(ConsoleWindow), 0, true);
            
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }
            
            var windowFrame = (IVsWindowFrame)window.Frame;
            VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }


        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the tool window
                CommandID toolwndFoldersID = new CommandID(GuidList.guidVSFoldersCmdSet, (int)PkgCmdIDList.cmdidFolders);
                CommandID toolwndCommandID = new CommandID(GuidList.guidVSFoldersCmdSet, (int)PkgCmdIDList.cmdidConsole);
                MenuCommand menuFoldersWin = new MenuCommand(ShowToolFoldersWindow, toolwndFoldersID);
                MenuCommand menuConsoleWin = new MenuCommand(ShowToolConsoleWindow, toolwndCommandID);
                mcs.AddCommand(menuFoldersWin);
                mcs.AddCommand(menuConsoleWin);
            }

            LoadSettings();
            GetDialogPageInternal<GeneralSettingsPage>();
            Settings.PropertyChanged += SettingsOnPropertyChanged;

            if (Settings.OpenFolders == null)
            {
                Settings.OpenFolders = new ObservableCollection<string>();
            }

            if (Settings.ExpandedFolders == null)
            {
                Settings.ExpandedFolders = new ObservableCollection<string>();
            }

            Settings.OpenFolders.CollectionChanged += OpenFoldersOnCollectionChanged;
            Settings.ExpandedFolders.CollectionChanged += ExpandedFoldersOnCollectionChanged;
        }

        private static void ExpandedFoldersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            SaveSettings();
        }

        private static void OpenFoldersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            SaveSettings();
        }

        private static void SettingsOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            SaveSettings();
        }

        #endregion

        public static FoldersWindow DemandFoldersWindow()
        {
            return (FoldersWindow) _instance.FindToolWindow(typeof (FoldersWindow), 0, true);
        }

        public static SettingsModel Settings { get; private set; }

        private static string GetSettingsFilePath()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var path = assembly.Location;
            path = Path.GetDirectoryName(path);
            string file = Path.Combine(path, "Settings.json");
            return file;
        }
        public static void LoadSettings()
        {
            string file = GetSettingsFilePath();
            var settingsText = File.Exists(file) ? File.ReadAllText(file) : "{}";
            Settings = JsonConvert.DeserializeObject<SettingsModel>(settingsText);
        }

        public static void SaveSettings()
        {
            if (!Settings.SuppressSave)
            {
                string file = GetSettingsFilePath();
                File.WriteAllText(file, JsonConvert.SerializeObject(Settings));
            }
        }

        public static ConsoleWindow GetConsoleWindow(bool demand)
        {
            var existing = (ConsoleWindow) _instance.FindToolWindow(typeof (ConsoleWindow), 0, true);
            var windowFrame = (IVsWindowFrame) existing.Frame;
            VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
            return existing;
        }

        public static void FireSettingsChanged()
        {
            var handler = SettingsChanged;

            if (handler != null)
            {
                handler(_instance, new EventArgs());
            }
        }

        public static event EventHandler SettingsChanged;
    }
}
