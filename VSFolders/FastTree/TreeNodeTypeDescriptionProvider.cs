using System;
using System.ComponentModel;

namespace Microsoft.VSFolders.FastTree
{
    public class TreeNodeTypeDescriptionProvider : TypeDescriptionProvider
    {
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new TreeNodeTypeDescriptor(instance as ITreeNode);
        }
    }
}