using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.VSFolders.Build
{
    internal static class FileUtils
    {
        public static IEnumerable<string> GetFilesRelative(string path, string filter, SearchOption option = SearchOption.AllDirectories)
        {
            return Directory.GetFiles(path, filter, option).Select(x => x.Substring(path.Length));
        }

        public static void ForEach<T>(this IEnumerable<T> list, Action<T> act)
        {
            foreach (var obj in list)
                act(obj);
        }
    }
}
