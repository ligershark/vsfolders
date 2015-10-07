using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using Microsoft.VSFolders.FastTree;

namespace Microsoft.VSFolders.Models
{
    public class FileSystemTracker
    {
        private readonly TreeCollection<FileData> _root;
        private readonly ConcurrentDictionary<string, FileData> _files;
        readonly Dispatcher _dispatcher;
        private readonly Dictionary<string, FileSystemWatcher> _watchers = new Dictionary<string, FileSystemWatcher>();

        private readonly object _syncRoot = new object();

        public FileSystemTracker(TreeCollection<FileData> root, ConcurrentDictionary<string, FileData> files)
        {
            _root = root;
            _files = files;
            _dispatcher = Dispatcher.CurrentDispatcher;
            _root.CollectionChanged += FileData_CollectionChanged;
        }


        void FileData_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case (NotifyCollectionChangedAction.Add):
                    {
                        foreach (var file in e.NewItems.Cast<TreeNode<FileData>>())
                            AddWatcher(file.Value.FullPath);
                        break;
                    }

                case (NotifyCollectionChangedAction.Remove):
                    {
                        foreach (var file in e.OldItems.Cast<TreeNode<FileData>>())
                            RemoveWatcher(file.Value.FullPath);
                        break;
                    }

            }
        }

        private void AddWatcher(string path)
        {
            if (!Directory.Exists(path))
                return;
            if (_watchers.ContainsKey(path))
                return;
            var watch = new FileSystemWatcher { Path = path, IncludeSubdirectories = true };
            watch.Created += watch_Created;
            watch.Deleted += watch_Deleted;
            watch.Renamed += watch_Renamed;
            watch.Changed += watch_Changed;
            watch.EnableRaisingEvents = true;
            _watchers.Add(path, watch);
        }

        private void watch_Changed(object sender, FileSystemEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Changed " + e.FullPath);
            lock (_syncRoot)
            {
                _dispatcher.Invoke(() =>
                {
                    FileData data;
                    if (_files.TryGetValue(e.FullPath, out data))
                    {
                        data.SetFullPath(e.FullPath);
                        data.RefreshValues();
                    }

                    if (VSFoldersPackage.Settings.OpenFolders.Contains(e.FullPath))
                    {
                        VSFoldersPackage.Settings.OpenFolders.Remove(e.FullPath);
                        VSFoldersPackage.Settings.OpenFolders.Add(e.FullPath);
                    }
                });
            }
        }

        private void RemoveWatcher(string path)
        {
            FileSystemWatcher watch;
            if (_watchers.TryGetValue(path, out watch))
            {
                watch.Created -= watch_Created;
                watch.Deleted -= watch_Deleted;
                watch.Renamed -= watch_Renamed;
                watch.Changed -= watch_Changed;
                _watchers.Remove(path);
            }
        }

        void watch_Renamed(object sender, RenamedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Renamed " + e.OldFullPath + " : " + e.FullPath);

            lock (_syncRoot)
            {
                _dispatcher.Invoke(() =>
                {
                    FileData data;
                    if (_files.TryGetValue(e.OldFullPath, out data))
                    {
                        data.SetFullPath(e.FullPath);
                        data.RefreshValues();
                    }

                    if (VSFoldersPackage.Settings.OpenFolders.Contains(e.OldFullPath))
                    {
                        VSFoldersPackage.Settings.OpenFolders.Remove(e.OldFullPath);
                        VSFoldersPackage.Settings.OpenFolders.Add(e.FullPath);
                    }
                });
            }
        }

        void watch_Deleted(object sender, FileSystemEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Deleted " + e.FullPath);

            _dispatcher.Invoke(() =>
            {
                lock (_syncRoot)
                {
                    FileData file;
                    if (_files.TryGetValue(e.FullPath, out file))
                    {
                        if (file == null)
                        {
                            return;
                        }

                        file.Cleanup();

                        if (file.Parent != null && file.Parent.TreeNode != null)
                        {
                            file.Parent.TreeNode.RemoveWhere(x => Equals(x.Value, file));
                        }
                    }
                }
            });
        }

        void watch_Created(object sender, FileSystemEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Created " + e.FullPath);

            lock (_syncRoot)
            {
                _dispatcher.Invoke(() =>
                {
                    string parentPath = TryGetDirectoryName(e.FullPath);
                    if (parentPath != null)
                    {
                        FileData parent;
                        if (_files.TryGetValue(parentPath, out parent))
                        {
                            parent.TreeNode.Add(new FileData(parent, e.FullPath, _files));
                        }
                    }
                });
            }
        }

        private string TryGetDirectoryName(string path)
        {
            try
            {
                return Path.GetDirectoryName(path);
            }
            catch (Exception)
            {

            }

            return null;
        }


    }
}
