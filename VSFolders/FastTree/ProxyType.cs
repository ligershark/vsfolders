using System;
using System.Reflection;

namespace Microsoft.VSFolders.FastTree
{
    public class ProxyType<T> : ICustomTypeProvider
    {
        public Type GetCustomType()
        {
            return typeof (T);
        }
    }
}