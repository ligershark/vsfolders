namespace Microsoft.VSFolders
{
    using System;
    using System.Collections.Generic;
    using VisualStudio;
    using VisualStudio.TextManager.Interop;

    public class FindScope : IVsFindScope
    {
        private readonly string name;

        private readonly Func<IEnumerable<string>> pathProvider;

        public FindScope(string name, Func<IEnumerable<string>> pathProvider)
        {
            this.name = name;
            this.pathProvider = pathProvider;
        }

        public int GetUIName(out string pbsName)
        {
            pbsName = this.name;
            return VSConstants.S_OK;
        }

        public int GetQuery(out string pbstrBaseDirectory, out string pbstrQuery)
        {
            pbstrQuery = string.Join(";", this.pathProvider());
            pbstrBaseDirectory = null;
            return VSConstants.S_OK;
        }

        public int EnumFilenames(out VisualStudio.OLE.Interop.IEnumString ppEnumString)
        {
            // this method doesn't work.  no matter what I do, EnumString throws
            // a null ref inside of Next
            ppEnumString = new EnumString(this.pathProvider);
            return VSConstants.E_NOTIMPL;
        }
    }
}