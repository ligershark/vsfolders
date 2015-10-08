// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileData.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   FileData.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using Services;
    using ShellIcons;

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class FileData : ObservableType
    {
        public static readonly ComparerOf<FileData> Comparer =
            new ComparerOf<FileData>().OnDescending(x => x.IsDirectory).On(x => x.Name);

        private readonly object _sync = new object();

        private string _fullPath;

        private string _name;

        private Lazy<BitmapSource> _icon;

        private SortedObservableCollection<FileData> _children;

        private bool _isExpanded;

        private bool _isHidden;

        private bool _isSelected;

        public FileData(FileData parent, string selectedPath)
        {
            this.Parent = parent;
            this.SetFullPath(selectedPath);
        }

        public bool IsDirectory { get; private set; }

        public FileData Parent { get; private set; }

        public string Name
        {
            get { return this._name; }
            protected set { this.Set(ref this._name, value); }
        }

        public bool AreChildrenLoaded
        {
            get
            {
                lock (this._sync)
                {
                    return this._children != null;
                }
            }
        }

        public SortedObservableCollection<FileData> Children
        {
            get
            {
                if (this._children == null && this.IsDirectory)
                {
                    lock (this._sync)
                    {
                        if (this._children == null)
                        {
                            this._children = new SortedObservableCollection<FileData>(
                                this.LoadChildFileSystemInfos(),
                                FileData.Comparer,
                                null);
                        }
                    }
                }

                return this._children;
            }
        }

        public Lazy<BitmapSource> Icon
        {
            get { return this._icon; }
            set { this.Set(ref this._icon, value); }
        }

        public string FullPath
        {
            get { return this._fullPath; }
            private set { this.Set(ref this._fullPath, value); }
        }

        public bool IsExpanded
        {
            get { return this._isExpanded; }
            set { this.Set(ref this._isExpanded, value); }
        }

        public bool IsSelected
        {
            get { return this._isSelected; }
            set
            {
                this.Set(ref this._isSelected, value);
                if (value)
                {
                    Factory.Resolve<FolderService>().SelectedItem = this;
                }
            }
        }

        public bool IsHidden
        {
            get { return this._isHidden; }
            set { this.Set(ref this._isHidden, value); }
        }

        public void Add(string child)
        {
            lock (this._sync)
            {
                if (this.AreChildrenLoaded)
                {
                    if (this.Children.FirstOrDefault(x => x.FullPath == child) != null)
                    {
                        return;
                    }

                    this.Children.AddLocal(new FileData(this, child));
                }
            }
        }

        public void Remove(FileData child)
        {
            if (this.AreChildrenLoaded)
            {
                this.Children.RemoveLocal(child);
            }
        }

        public void SetFullPath(string path)
        {
            if (this.FullPath != null)
            {
                FileData outs;
                Factory.Resolve<FolderService>().Files.TryRemove(this.FullPath, out outs);
            }

            this.FullPath = path;

            Factory.Resolve<FolderService>().Files.TryAdd(this.FullPath, this);

            this.IsDirectory = Directory.Exists(path);
            if (this.IsDirectory)
            {
                this.Name = new DirectoryInfo(path).Name;
            }
            else
            {
                FileInfo info = new FileInfo(path);
                this.Name = info.Name;
                this.IsHidden = info.Attributes.HasFlag(FileAttributes.Hidden);
            }

            this.Icon = new Lazy<BitmapSource>(() => IconUtil.GetIcon(this.FullPath));
        }

        public void Cleanup()
        {
            if (this.AreChildrenLoaded)
            {
                this.Children.ForEach(x => x.Cleanup());
            }

            var settings = Factory.Resolve<Settings>();
            if (settings.OpenFolders.Contains(this._fullPath))
            {
                settings.OpenFolders.Remove(this._fullPath);
            }

            FileData outs;
            Factory.Resolve<FolderService>().Files.TryRemove(this._fullPath, out outs);
            this._children = null;
        }

        private string DebuggerDisplay
        {
            get { return $"{this.Name} ({(this.IsDirectory ? "Directory" : "File")}: {this.FullPath})"; }
        }

        private IEnumerable<FileData> LoadChildFileSystemInfos()
        {
            if (this.IsDirectory)
            {
                foreach (string entry in Directory.GetFileSystemEntries(this.FullPath))
                {
                    yield return new FileData(this, entry);
                }
            }
        }
    }
}