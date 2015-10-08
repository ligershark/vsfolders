namespace Microsoft.VSFolders
{
    using System;
    using System.Collections.Generic;
    using VisualStudio;

    internal sealed class EnumString : VisualStudio.OLE.Interop.IEnumString
    {
        private readonly Func<IEnumerable<string>> strings;

        private readonly IEnumerator<string> enumerator;

        public EnumString()
        {

        }

        public EnumString(Func<IEnumerable<string>> strings)
        {
            this.strings = strings;
            this.enumerator = strings().GetEnumerator();
        }

        public void Clone(out VisualStudio.OLE.Interop.IEnumString ppenum)
        {
            ppenum = new EnumString(this.strings);
        }

        int VisualStudio.OLE.Interop.IEnumString.Next(
            uint celt,
            string[] rgelt,
            out uint pceltFetched)
        {
            int fetched = 0;

            while (celt > 0 && this.enumerator.MoveNext())
            {
                rgelt[fetched] = this.enumerator.Current;
                fetched++;
                celt--;
            }

            pceltFetched = (uint)fetched;

            return celt == 0 ? VSConstants.S_OK : VSConstants.S_FALSE;
        }

        public int Reset()
        {
            this.enumerator.Reset();
            return VSConstants.S_OK;
        }

        public int Skip(uint celt)
        {
            for (int i = 0; i < celt; i++)
            {
                if (!this.enumerator.MoveNext())
                {
                    return VSConstants.S_FALSE;
                }
            }

            return VSConstants.S_OK;
        }
    }
}