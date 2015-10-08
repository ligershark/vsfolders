// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FoldersWindowViewModel.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   FoldersWindowViewModel.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    using Commands;
    using ContextMenu;
    using Services;

    public class FoldersWindowViewModel : ObservableType
    {
        private readonly Lazy<List<MenuItemInfo>> _menuItems;

        private bool _isUpdating;

        public FoldersWindowViewModel()
        {
            this.DataSource = Factory.Resolve<FolderService>();
            this.BrowseForFolderCommand = new BrowseForFolderCommand(new AddFolderCommand().Execute);
            this._menuItems = new Lazy<List<MenuItemInfo>>(FoldersWindowViewModel.CreateMenuItemDefinitions);
            Factory.Resolve<ContextMenuService>().RegisterHandler<FileData>(this.GetMenus);

            this.InitFolders();
        }

        public ActionCommand OpenItemCommand
        {
            get { return new ActionCommand(x => this.OpenItem(x as FileData)); }
        }

        public ActionCommand PreviewItemCommand
        {
            get { return new ActionCommand(x => this.ShowPreview(x as FileData)); }
        }

        public FolderService DataSource { get; private set; }

        public ICommand BrowseForFolderCommand { get; private set; }

        public bool IsUpdating
        {
            get { return this._isUpdating; }
            set
            {
                this._isUpdating = value;
                this.OnPropertyChanged();
            }
        }

        private static List<MenuItemInfo> CreateMenuItemDefinitions()
        {
            MenuItemInfo addmenu = new MenuItemInfo("Add", o => o.IsDirectory, null);
            List<MenuItemInfo> items = new List<MenuItemInfo>
            {
                new MenuItemInfo(
                    "Open Solution",
                    o => o.FullPath.EndsWith(".sln", StringComparison.OrdinalIgnoreCase),
                    new OpenSolutionCommand()),
                new MenuItemInfo(
                    "Open Project",
                    o => Regex.IsMatch(o.FullPath, ".*\\..*proj"),
                    new OpenProjectCommand()),
                new MenuItemInfo(
                    "Exclude from Folder Explorer",
                    o => o.IsDirectory && o.Parent == null,
                    new RemoveFolderCommand()),
                new MenuItemInfo("Open in Windows Explorer", o => true, new BrowseCommand()),
                MenuItemInfo.Empty,
                addmenu,
                MenuItemInfo.Empty,
                new MenuItemInfo("Delete", o => true, new DeleteCommand()),
                new MenuItemInfo("Rename", o => true, new RenameCommand())
            };

            addmenu.Children = new List<MenuItemInfo>
            {
                new MenuItemInfo("New Folder", o => o.IsDirectory, new CreateFolderCommand()),
                new MenuItemInfo("New File", o => o.IsDirectory, new CreateFileCommand())
            };

            return items;
        }

        public void ShowPreview(FileData data)
        {
            if (this.IsUpdating || data == null)
            {
                return;
            }

            try
            {
                this.IsUpdating = true;
                string fullPath = data.FullPath;
                Factory.Resolve<OpenInPreviewCommand>().Execute(fullPath);
            }
            finally
            {
                this.IsUpdating = false;
            }
        }

        public void OpenItem(FileData data)
        {
            if (this.IsUpdating || data?.FullPath == null)
            {
                return;
            }

            Factory.Resolve<OpenFileCommand>().Execute(data.FullPath);
        }

        private IEnumerable<IContextMenuItem> GetMenus(object obj)
        {
            FileData fd = (FileData)obj;
            List<IContextMenuItem> workingList = new List<IContextMenuItem>(this._menuItems.Value.Count);
            MenuItemInfo last = null;
            foreach (MenuItemInfo item in this._menuItems.Value)
            {
                if (item == MenuItemInfo.Empty && last == MenuItemInfo.Empty)
                {
                    continue;
                }

                if (item.Predicate != null && !item.Predicate(fd))
                {
                    continue;
                }

                workingList.Add(item.Convert());
                last = item;
            }

            return workingList;
        }

        private void InitFolders()
        {
            List<string> toRemove = new List<string>();
            HashSet<string> added = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            AddFolderCommand cmd = new AddFolderCommand();

            var settings = Factory.Resolve<Settings>();
            foreach (string openFolder in settings.OpenFolders)
            {
                if (!added.Contains(openFolder) && Directory.Exists(openFolder))
                {
                    cmd.Execute(openFolder);
                    added.Add(openFolder);
                }
                else
                {
                    toRemove.Add(openFolder);
                }
            }

            foreach (string folder in toRemove)
            {
                settings.OpenFolders.Remove(folder);
            }
        }
    }
}