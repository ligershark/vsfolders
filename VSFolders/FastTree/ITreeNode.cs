using System;
using System.Collections.Generic;

namespace Microsoft.VSFolders.FastTree
{
    public interface ITreeNode : ITreeCollection
    {
        IEnumerable<ITreeNode> Children { get; }

        object Value { get; set; }

        bool MatchesFilter { get; }

        Type ElementType { get; }
        
        ITreeNode Self { get; }

        object this[string name] { get; set; }

        bool HasProperty(string name);
    }
}