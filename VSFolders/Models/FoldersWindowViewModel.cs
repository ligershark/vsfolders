using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.VSFolders.Commands;
using Microsoft.VSFolders.Converters;
using Microsoft.VSFolders.FastTree;
using Microsoft.VSFolders.Settings;
using Microsoft.VSFolders.ViewModels;

namespace Microsoft.VSFolders.Models
{
    public class FoldersWindowViewModel : BindableType
    {
        private readonly GestureHandler _gestures;

        private readonly Lazy<List<MenuItemInfo>> _menuItems;

        private bool _isUpdating;

        private string _path;

        private string _searchText;

        public event FolderChangedEventHandler FolderChanged;

        public FolderDataSource DataSource { get; private set; }

        public ICommand BrowseForFolderCommand { get; private set; }
        public ICommand ClearSearchCommand { get; private set; }
        public ICommand LaunchSettingsCommand { get; private set; }
        public ICommand PerformSearchCommand { get; private set; }

        public FoldersWindowViewModel()
        {
            DataSource = ServiceContainer.Resolve<FolderDataSource>();
            PerformSearchCommand = new ActionCommand(o => DataSource.Tree.ApplyFilter(n => Filter(n.Value)));
            ClearSearchCommand = new ActionCommand(o => SearchText = string.Empty);
            BrowseForFolderCommand = new BrowseForFolderCommand(AddPath);
            LaunchSettingsCommand = new ActionCommand(o => VSFoldersPackage.ShowDialogPage<GeneralSettingsPage>());
            DataSource.Tree.PropertyChanged += PropagateChanges;
            VSFoldersPackage.Settings.PropertyChanged += UpdateFilters;
            
            _gestures = new GestureHandler(this);

            _menuItems = new Lazy<List<MenuItemInfo>>(CreateMenuItemDefinitions);
            ServiceContainer.Resolve<ContextMenuService>().RegisterHandler<TreeNode<FileData>>(GetMenus);

            InitFolders();
        }

        private static List<MenuItemInfo> CreateMenuItemDefinitions()
        {
            var addmenu = new MenuItemInfo("Add", o => o.IsDirectory, null);
            var iisEx = new LaunchIISExpressCommand();
            var items = new List<MenuItemInfo>
            {
                new MenuItemInfo("Open Solution", o => o.FullPath.EndsWith(".sln", StringComparison.OrdinalIgnoreCase), new OpenSolutionCommand()),
                new MenuItemInfo("Open Project", o => Regex.IsMatch(o.FullPath, ".*\\..*proj"), new OpenProjectCommand()),
                new MenuItemInfo("Exclude from Folder Explorer", o=>o.IsDirectory && o.Parent == null, new RemoveCommand()),
                new MenuItemInfo("Open in Windows Explorer", o=>true, new BrowseCommand()),
                new MenuItemInfo("Open Console Here", o => true, new OpenConsoleCommand()),
                MenuItemInfo.Empty,
                addmenu,
                MenuItemInfo.Empty,
                new MenuItemInfo("Build", o=>o.IsDirectory, new BuildCommand(), "Build"),
                new MenuItemInfo("Shell Menu", o=>true, new ShowShellMenuCommand()),
                new MenuItemInfo("Launch IIS Express", iisEx.CanExecute, iisEx),
                MenuItemInfo.Empty,
                new MenuItemInfo("Delete", o=>true, new DeleteCommand()),
                new MenuItemInfo("Rename", o=>true, new RenameCommand()),
            };

            addmenu.Children = new List<MenuItemInfo>
            {
                new MenuItemInfo("New Folder", o => o.IsDirectory, new AddFolderCommand()),
                new MenuItemInfo("New File", o => o.IsDirectory, new AddFileCommand())
            };

            return items;
        }

        private IEnumerable<IContextMenuItem> GetMenus(object obj)
        {
            var fd = ((TreeNode<FileData>)obj).Value;
            var workingList = new List<IContextMenuItem>(_menuItems.Value.Count);
            MenuItemInfo last = null;
            foreach (var item in _menuItems.Value)
            {
                if (item == MenuItemInfo.Empty && last == MenuItemInfo.Empty)
                    continue;
                if (item.Predicate != null && !item.Predicate(fd))
                    continue;
                workingList.Add(item.Convert());
                last = item;
            }
            return workingList;
        }

        public FileData Current
        {
            get
            {
                if (Path == null)
                    return null;
                FileData data;
                DataSource.Files.TryGetValue(Path, out data);
                return data;
            }
        }

        public GestureHandler Gestures
        {
            get { return _gestures; }
        }

        public bool IsSearching
        {
            get { return !string.IsNullOrEmpty(SearchText); }
        }

        public bool IsUpdating
        {
            get { return DataSource.Tree.IsSearching || _isUpdating; }
            set
            {
                _isUpdating = value;
                OnPropertyChanged();
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                if (_path != value)
                {
                    _path = value;
                    OnPropertyChanged();
                    OnFolderChanged(_path);
                }
            }
        }
        
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                DataSource.Tree.ExpandOnFilterMatch = !string.IsNullOrEmpty(value);
                _searchText = value;
                OnPropertyChanged();
                OnPropertyChanged("IsSearching");
                DataSource.Tree.ApplyFilter(n => Filter(n.Value));
            }
        }

        public void AddPath(string path)
        {
            if (path == null)
                return;
            if (Directory.Exists(path) && !DataSource.Files.ContainsKey(path))
            {
                DataSource.Tree.Add(new FileData(null, path, DataSource.Files));

                if (!VSFoldersPackage.Settings.OpenFolders.Contains(path))
                {
                    VSFoldersPackage.Settings.OpenFolders.Add(path);
                }

                Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() => DataSource.Tree.ApplyFilter(n => Filter(n.Value))));
            }
        }

        internal bool Filter(FileData data)
        {
            return FilterAsync(data).Result;
        }

        internal async Task<bool> FilterAsync(FileData data)
        {
            data.RefreshValues();

            if (!VSFoldersPackage.Settings.ShowHiddenFiles)
            {
                if (data.IsHidden)
                {
                    return false;
                }

                try
                {
                    if (FileDataToEffectivelyHiddenIndicationConverter.IsEffectivelyHidden(data))
                    {
                        return false;
                    }
                }
                catch
                {
                    System.Diagnostics.Trace.WriteLine(string.Format("FilterAsync failed for {0}", data.FullPath));
                }
            }

            if (string.IsNullOrEmpty(SearchText))
            {
                return true;
            }

            if (data.Name.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) > -1)
            {
                return true;
            }

            if (VSFoldersPackage.Settings.SearchInFiles)
            {
                var result = await Task.Run(() => SearchInFile(data));

                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual void OnFolderChanged(string newFolder)
        {
            var handler = FolderChanged;

            if (handler != null)
            {
                var toPath = Directory.Exists(_path) ? _path : System.IO.Path.GetDirectoryName(_path);

                try
                {
                    handler(this, new FolderChangedEventHandlerArgs(toPath));
                }
                catch
                {
                    System.Diagnostics.Trace.WriteLine(string.Format("FolderChanged handler failed for {0}", newFolder));
                }
            }
        }

        private void InitFolders()
        {
            var toRemove = new List<string>();
            var added = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var openFolder in VSFoldersPackage.Settings.OpenFolders)
            {
                if (!added.Contains(openFolder) && Directory.Exists(openFolder))
                {
                    var data = new FileData(null, openFolder, DataSource.Files);
                    DataSource.Tree.Add(data);
                    added.Add(openFolder);
                }
                else
                {
                    toRemove.Add(openFolder);
                }
            }

            foreach (var folder in toRemove)
            {
                VSFoldersPackage.Settings.OpenFolders.Remove(folder);
            }

            DataSource.Tree.ApplyFilter(x => Filter(x.Value));
        }

        private void PropagateChanges(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSearching")
            {
                OnPropertyChanged("IsUpdating");
            }
        }

        private bool SearchInFile(FileData data)
        {
            try
            {
                if (data.IsDirectory)
                {
                    return false;
                }

                var fi = new FileInfo(data.FullPath);
                if (fi.Length < 1024 * 1024)
                {
                    return File.ReadAllText(fi.FullName).IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) > -1;
                }
            }
            catch
            {
                System.Diagnostics.Trace.WriteLine(string.Format("SearchInFile failed for {0}", data.FullPath));
            }

            return false;
        }

        private void UpdateFilters(object sender, EventArgs e)
        {
            DataSource.Tree.ApplyFilter(n => Filter(n.Value));
            DataSource.Tree.RefreshAll();
        }
    }
}
