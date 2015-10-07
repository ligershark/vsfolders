using System;
using System.Collections.Concurrent;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.VSFolders.Build;
using Microsoft.VSFolders.FastTree;
using Microsoft.VSFolders.ShellIcons;

namespace Microsoft.VSFolders.Models
{
    public class FileData : BindableType, IFastTreeEnlightened<FileData>
    {
        private readonly ConcurrentDictionary<string, FileData> _files;
        private string _fullPath;
        private string _name;
        private BitmapSource _icon;

        public bool IsDirectory { get; private set; }

        public FileData Parent { get; private set; }

        public string Name
        {
            get { return _name; }
            protected set { Set(ref _name, value); }
        }

        public BitmapSource Icon
        {
            get { return _icon; }
            set { Set(ref _icon, value); }
        }

        public FileData(FileData parent, string selectedPath, ConcurrentDictionary<string, FileData> files)
        {
            //BeginRenameCommand = new ActionCommand(BeginRename);
            //CancelRenameCommand = new ActionCommand(CancelRename);
            //CommitRenameCommand = new ActionCommand(CommitRename);

            Parent = parent;
            _files = files;

            SetFullPath(selectedPath);
            if (VSFoldersPackage.Settings.ExpandedFolders.Contains(selectedPath))
            {
                IsExpanded = true;
            }

            RefreshValues();
        }

        public void CommitRename()
        {
            if (String.IsNullOrEmpty(TempName))
                return;

            if (IsDirectory)
            {
                Directory.Move(_fullPath, Path.Combine(Path.GetDirectoryName(_fullPath), TempName));
            }
            else
            {
                File.Move(_fullPath, Path.Combine(Path.GetDirectoryName(_fullPath), TempName));
            }
        }

        private void CancelRename()
        {
            IsRenaming = false;
        }

        private void BeginRename()
        {
            IsRenaming = true;
        }

        public void SetFullPath(string path)
        {
            if (_fullPath != null)
            {
                FileData outs;
                _files.TryRemove(_fullPath, out outs);
            }

            _fullPath = path;

            _files.TryAdd(_fullPath, this);
            IsDirectory = Directory.Exists(path);
            Name = (IsDirectory ? (FileSystemInfo)new DirectoryInfo(path) : new FileInfo(path)).Name;
            Icon = IconUtil.RefreshIcon(path);
        }

        public void Cleanup()
        {
            TreeNode.ForEach<TreeNode<FileData>>(x => x.Value.Cleanup());

            if (VSFoldersPackage.Settings.OpenFolders.Contains(_fullPath))
            {
                VSFoldersPackage.Settings.OpenFolders.Remove(_fullPath);
            }
            FileData outs;
            _files.TryRemove(_fullPath, out outs);
        }

        public string FullPath { get { return _fullPath; } }

        private bool _isExpanded;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                OnPropertyChanged();

                if (value)
                {
                    VSFoldersPackage.Settings.ExpandedFolders.Add(FullPath);
                }
                else
                {
                    VSFoldersPackage.Settings.ExpandedFolders.Remove(FullPath);
                }
            }
        }

        public string TempName
        {
            get { return _tempName; }
            set
            {
                _tempName = value;
                OnPropertyChanged();
            }
        }

        private bool _isRenaming;
        private string _tempName;
        private bool _isFilterExpanded;
        private bool _isHidden;

        public bool IsRenaming
        {
            get { return _isRenaming; }
            set
            {
                _isRenaming = value;
                OnPropertyChanged();

                if (value)
                {
                    TempName = Name;
                }
            }
        }

        //public ICommand BeginRenameCommand { get; private set; }
        //public ICommand CommitRenameCommand { get; private set; }
        //public ICommand CancelRenameCommand { get; private set; }

        public bool IsFilterExpanded
        {
            get { return _isFilterExpanded; }
            set
            {
                _isFilterExpanded = value;
                OnPropertyChanged();
                OnPropertyChanged("IsExpanded");
            }
        }

        public void RefreshValues()
        {
            if (IsDirectory)
            {
                IsHidden = new DirectoryInfo(FullPath).Attributes.HasFlag(FileAttributes.Hidden);
                return;
            }

            IsHidden = new FileInfo(FullPath).Attributes.HasFlag(FileAttributes.Hidden);
        }

        public bool IsHidden
        {
            get
            {
                return _isHidden;
            }
            set
            {
                _isHidden = value;
                OnPropertyChanged();
            }
        }

        public TreeNode<FileData> TreeNode { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1}: {2})", Name, IsDirectory ? "Directory" : "File", FullPath);
        }
    }
}