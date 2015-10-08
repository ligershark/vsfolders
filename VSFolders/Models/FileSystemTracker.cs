// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemTracker.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   FileSystemTracker.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Models
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows.Threading;

    public class FileSystemTracker
    {
        private readonly SortedObservableCollection<FileData> _root;

        private readonly ConcurrentDictionary<string, FileData> _files;

        private readonly Dispatcher _dispatcher;

        private readonly Dictionary<string, FileSystemWatcher> _watchers = new Dictionary<string, FileSystemWatcher>();

        private readonly object _syncRoot = new object();

        public FileSystemTracker(
            SortedObservableCollection<FileData> root, 
            ConcurrentDictionary<string, FileData> files)
        {
            this._root = root;
            this._files = files;
            this._dispatcher = Dispatcher.CurrentDispatcher;
            this._root.CollectionChanged += this.FileData_CollectionChanged;
        }

        private void FileData_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    foreach (FileData file in e.NewItems.Cast<FileData>())
                    {
                        this.AddWatcher(file.FullPath);
                    }

                    break;
                }

                case NotifyCollectionChangedAction.Remove:
                {
                    foreach (FileData file in e.OldItems.Cast<FileData>())
                    {
                        this.RemoveWatcher(file.FullPath);
                    }

                    break;
                }
            }
        }

        private void AddWatcher(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            if (this._watchers.ContainsKey(path))
            {
                return;
            }

            FileSystemWatcher watch = new FileSystemWatcher {Path = path, IncludeSubdirectories = true};
            watch.Created += this.watch_Created;
            watch.Deleted += this.watch_Deleted;
            watch.Renamed += this.watch_Renamed;
            watch.Changed += this.watch_Changed;
            watch.EnableRaisingEvents = true;
            this._watchers.Add(path, watch);
        }

        private void watch_Changed(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine("Changed " + e.FullPath);
            lock (this._syncRoot)
            {
                this._dispatcher.Invoke(
                    () =>
                    {
                        FileData data;
                        if (this._files.TryGetValue(e.FullPath, out data))
                        {
                            data.SetFullPath(e.FullPath);

                            // data.RefreshValues();
                        }

                        var settings = Factory.Resolve<Settings>();
                        if (settings.OpenFolders.Contains(e.FullPath))
                        {
                            settings.OpenFolders.Remove(e.FullPath);
                            settings.OpenFolders.Add(e.FullPath);
                        }
                    });
            }
        }

        private void RemoveWatcher(string path)
        {
            FileSystemWatcher watch;
            if (this._watchers.TryGetValue(path, out watch))
            {
                watch.Created -= this.watch_Created;
                watch.Deleted -= this.watch_Deleted;
                watch.Renamed -= this.watch_Renamed;
                watch.Changed -= this.watch_Changed;
                this._watchers.Remove(path);
            }
        }

        private void watch_Renamed(object sender, RenamedEventArgs e)
        {
            Debug.WriteLine("Renamed " + e.OldFullPath + " : " + e.FullPath);

            lock (this._syncRoot)
            {
                this._dispatcher.Invoke(
                    () =>
                    {
                        FileData data;
                        if (this._files.TryGetValue(e.OldFullPath, out data))
                        {
                            data.SetFullPath(e.FullPath);

                            // data.RefreshValues();
                        }

                        var settings = Factory.Resolve<Settings>();
                        if (settings.OpenFolders.Contains(e.OldFullPath))
                        {
                            settings.OpenFolders.Remove(e.OldFullPath);
                            settings.OpenFolders.Add(e.FullPath);
                        }
                    });
            }
        }

        private void watch_Deleted(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine("Deleted " + e.FullPath);

            lock (this._syncRoot)
            {
                this._dispatcher.Invoke(
                    () =>
                    {
                        FileData file;
                        if (this._files.TryGetValue(e.FullPath, out file))
                        {
                            if (file == null)
                            {
                                return;
                            }

                            file.Cleanup();

                            if (file.Parent != null && file.Parent.AreChildrenLoaded)
                            {
                                file.Parent.Remove(file);
                            }
                        }
                    });
            }
        }

        private void watch_Created(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine("Created " + e.FullPath);

            lock (this._syncRoot)
            {
                this._dispatcher.Invoke(
                    () =>
                    {
                        string parentPath = this.TryGetDirectoryName(e.FullPath);
                        if (parentPath != null)
                        {
                            FileData parent;
                            if (this._files.TryGetValue(parentPath, out parent))
                            {
                                parent.Add(e.FullPath);
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