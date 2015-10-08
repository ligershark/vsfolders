// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchResult.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   SearchResult.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Search
{
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;

    public class SearchResult
    {
        public string FilePath { get; set; }

        public long Location { get; set; }
    }

    public class SearchOperation
    {
        public SearchOperation()
        {
            this.Hits = new ObservableCollection<SearchResult>();
        }

        public string Path { get; set; }

        public string SearchString { get; set; }

        public string FileFilter { get; set; }

        public ObservableCollection<SearchResult> Hits { get; set; }

        public CancellationToken CancellationToken { get; set; }

        public Task SearchTask { get; set; }
    }
}