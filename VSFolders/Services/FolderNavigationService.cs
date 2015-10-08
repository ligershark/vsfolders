// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderNavigationService.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   FolderNavigationService.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.VSFolders.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Controls;
    using Models;

    public class FolderNavigationService
    {
        private static readonly PropertyInfo ItemsHostPropertyInfo = typeof(ItemsControl).GetProperty(
            "ItemsHost",
            BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo EnsureGeneratorMethodInfo = typeof(Panel).GetMethod(
            "EnsureGenerator",
            BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Gets or sets the TreeView.
        /// </summary>
        /// <value>
        /// The TreeView.
        /// </value>
        public TreeView TreeView { get; set; }

        /// <summary>
        /// Gets the items host.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns></returns>
        private static Panel GetItemsHost(ItemsControl itemsControl)
        {
            Debug.Assert(itemsControl != null);
            return FolderNavigationService.ItemsHostPropertyInfo.GetValue(itemsControl, null) as Panel;
        }

        /// <summary>
        /// Calls EnsureGenerator
        /// </summary>
        /// <param name="panel">The panel.</param>
        private static void CallEnsureGenerator(Panel panel)
        {
            Debug.Assert(panel != null);
            FolderNavigationService.EnsureGeneratorMethodInfo.Invoke(panel, null);
        }

        /// <summary>
        /// Navigates to.
        /// http://stackoverflow.com/questions/183636/selecting-a-node-in-virtualized-treeview-with-wpf
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><see cref="bool"/></returns>
        public bool NavigateTo(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            FolderService ds = Factory.Resolve<FolderService>();

            FileData root =
                ds.Tree.FirstOrDefault(
                    x => path.StartsWith(x.FullPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase));

            if (root == null)
            {
                return false;
            }

            string[] pathParts = FolderNavigationService.GetPathParts(path.Substring(root.FullPath.Length));
            List<FileData> tree = new List<FileData> { root };

            FileData current = root;
            for (int i = 0; i < pathParts.Length; i++)
            {
                FileData next = current.Children.FirstOrDefault(
                    x => x.IsDirectory == (i != pathParts.Length - 1) &&
                         string.Equals(x.Name, pathParts[i], StringComparison.OrdinalIgnoreCase));
                tree.Add(next);
                current = next;

                if (next == null)
                    return false;
            }

            ItemsControl currentParent = this.TreeView;
            foreach (FileData node in tree)
            {
                // first try the easy way
                TreeViewItem newParent = currentParent.ItemContainerGenerator.ContainerFromItem(node) as TreeViewItem;
                if (newParent == null)
                {
                    // if this failed, it's probably because of virtualization, and we will have to do it the hard way.
                    // this code is influenced by TreeViewItem.ExpandRecursive decompiled code, and the MSDN sample at http://code.msdn.microsoft.com/Changing-selection-in-a-6a6242c8/sourcecode?fileId=18862&pathId=753647475
                    // see also the question at http://stackoverflow.com/q/183636/46635
                    currentParent.ApplyTemplate();
                    ItemsPresenter itemsPresenter =
                        (ItemsPresenter)currentParent.Template.FindName("ItemsHost", currentParent);
                    if (itemsPresenter != null)
                    {
                        itemsPresenter.ApplyTemplate();
                    }
                    else
                    {
                        currentParent.UpdateLayout();
                    }

                    VirtualizingPanel virtualizingPanel =
                        FolderNavigationService.GetItemsHost(currentParent) as VirtualizingPanel;

                    if (virtualizingPanel == null)
                    {
                        return false;
                    }

                    FolderNavigationService.CallEnsureGenerator(virtualizingPanel);
                    int index = currentParent.Items.IndexOf(node);
                    if (index < 0)
                    {
                        throw new InvalidOperationException("Node '" + node + "' cannot be found in container");
                    }
                    virtualizingPanel.BringIndexIntoViewPublic(index);
                    newParent = currentParent.ItemContainerGenerator.ContainerFromIndex(index) as TreeViewItem;
                }

                if (newParent == null)
                {
                    throw new InvalidOperationException(
                        "Tree view item cannot be found or created for node '" + node + "'");
                }

                if (node == tree.Last())
                {
                    newParent.IsSelected = true;
                    newParent.BringIntoView();
                    break;
                }

                newParent.IsExpanded = true;
                currentParent = newParent;
            }
            return true;
        }

        /// <summary>
        /// Gets the path parts.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private static string[] GetPathParts(string path)
        {
            return path.Split(
                new[]
                {
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar
                },
                StringSplitOptions.RemoveEmptyEntries);
        }
    }
}