using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Microsoft.VSFolders.FastTree
{
    public interface ITreeCollection : IEnumerable<ITreeNode>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        int Count { get; }

        bool IsRoot { get; }

        void Clear();

        bool UseFiltering { get; set; }
    }
}