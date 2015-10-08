// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchProvider.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.VSFolders.Search
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Services;
    using VisualStudio.TextManager.Interop;

    /// <summary>
    /// The search provider.
    /// </summary>
    public class SearchProvider
    {
        /// <summary>
        /// The is initialized.
        /// </summary>
        private static bool isInitialized;

        /// <summary>
        /// The get files to search.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<string> GetFilesToSearch()
        {
            var ds = Factory.Resolve<FolderService>();
            return ds.Tree.Select(x => x.FullPath);
        }

        /// <summary>
        /// The get files to search.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<string> GetSelectedFolderToSearch()
        {
            var ds = Factory.Resolve<FolderService>();
            var item = ds.SelectedItem;

            if (item == null)
            {
                return Enumerable.Empty<string>();
            }

            if (item.IsDirectory)
            {
                return new[] {item.FullPath};
            }

            return new[] { item.Parent.FullPath };
        }

        public IEnumerable<string> GetFilesToSearch2()
        {
            var ds = Factory.Resolve<FolderService>();
            foreach (var f in ds.Tree.Select(x => x.FullPath))
            {
                foreach (var file in Directory.EnumerateFiles(f, "*.*", SearchOption.AllDirectories))
                {
                    yield return file;
                }
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        public void Initialize()
        {
            if (!SearchProvider.isInitialized)
            {
                IVsRegisterFindScope search = Factory.Resolve<IServiceProvider>()
                    .GetService(typeof(IVsRegisterFindScope)) as IVsRegisterFindScope;

                if (search != null)
                {
                    uint cookie;
                    search.RegisterFindScope(
                        new FindScope("Folders", Factory.Resolve<SearchProvider>().GetFilesToSearch),
                        out cookie);

                    search.RegisterFindScope(
                        new FindScope("Folders - Current Folder", Factory.Resolve<SearchProvider>().GetSelectedFolderToSearch),
                        out cookie);
                }

                SearchProvider.isInitialized = true;
            }
        }
    }
}