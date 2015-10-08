// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderService.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   FolderService.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Services
{
    using System;
    using System.Collections.Concurrent;
    using Models;

    public class FolderService : ObservableType
    {
        private ConcurrentDictionary<string, FileData> _files;

        private FileSystemTracker _tracker;

        private FileData _selectedItem;

        private SortedObservableCollection<FileData> _tree;

        public FolderService()
        {
            this.Files = new ConcurrentDictionary<string, FileData>(StringComparer.InvariantCultureIgnoreCase);
            this.Tree = new SortedObservableCollection<FileData>(FileData.Comparer);
            this._tracker = new FileSystemTracker(this.Tree, this._files);
        }

        public ConcurrentDictionary<string, FileData> Files
        {
            get { return this._files; }
            set { this.Set(ref this._files, value); }
        }

        public SortedObservableCollection<FileData> Tree
        {
            get { return this._tree; }
            set { this.Set(ref this._tree, value); }
        }

        public FileData SelectedItem
        {
            get { return this._selectedItem; }
            set { this.Set(ref this._selectedItem, value); }
        }
    }
}