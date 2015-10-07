using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.VSFolders.Models
{
    public static class Extensions
    {
        private static readonly object _syncRoot = new object();
        private static readonly StringComparer StringComparer = StringComparer.InvariantCultureIgnoreCase;
        public static void InsertInOrder(this IList<FileData> list, FileData obj)
        {
            //lock (_syncRoot)
            //{
                int order = GetSortedIndex(list, obj);
                list.Insert(order, obj);
            //}
        }

        private static int GetSortedIndex(IList<FileData> list, FileData obj)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (ReferenceEquals(list[i], obj))
                    continue;

                if (obj.IsDirectory != list[i].IsDirectory)
                {
                    if (obj.IsDirectory)
                    {
                        return Math.Max(0, i - 1);
                    }
                    continue;
                }

                if (StringComparer.Compare(list[i].Name, obj.Name) < 0)
                    continue;

                return i;
            }
            return Math.Max(0, list.Count - 1);
        }

        public static void FixOrder(this ObservableCollection<FileData> list, FileData obj)
        {
            //lock (_syncRoot)
            //{
                if (list.Count < 2)
                    return;
                int newIndex = GetSortedIndex(list, obj);
                int index = list.IndexOf(obj); //calculate this last just to make sure that the list doesn't change elsewhere
                list.Move(index, newIndex);
            //}
        }
    }
}
