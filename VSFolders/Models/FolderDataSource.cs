using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Microsoft.VSFolders.FastTree;

namespace Microsoft.VSFolders.Models
{
    public class FolderDataSource : BindableType
    {
        public ConcurrentDictionary<string, FileData> Files { get { return _files; } }
        private readonly ConcurrentDictionary<string, FileData> _files;
        private FileSystemTracker _tracker;
        public FolderDataSource()
        {
            _files = new ConcurrentDictionary<string, FileData>(StringComparer.InvariantCultureIgnoreCase);
            Tree = new TreeCollection<FileData>
            {
                Comparer = new ComparerOf<FileData>().OnDescending(x => x.IsDirectory).On(x => x.Name),
                LoadChildren = LoadChildFileSystemInfos,
                UseFiltering = true
            };
            _tracker = new FileSystemTracker(Tree, _files);
        }
        public TreeCollection<FileData> Tree { get; private set; }

        private IEnumerable<FileData> LoadChildFileSystemInfos(FileData data)
        {
            if (data.IsDirectory)
            {
                foreach (var entry in Directory.GetFileSystemEntries(data.FullPath))
                {
                    yield return new FileData(data, entry, _files);
                }
            }
        }


    }
}
