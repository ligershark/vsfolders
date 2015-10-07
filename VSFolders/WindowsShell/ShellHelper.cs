using System;

namespace Microsoft.VSFolders.WindowsShell
{
    internal static class ShellHelper
    {
        public static uint HiWord(IntPtr ptr)
        {
            if ((((int)ptr) & -2147483648) == -2147483648)
            {
                return (((uint)ptr) >> 0x10);
            }
            return (uint)((((int)ptr) >> 0x10) & 0xffff);
        }

        public static uint LoWord(IntPtr ptr)
        {
            return (uint)(((int)ptr) & 0xffff);
        }
    }
}