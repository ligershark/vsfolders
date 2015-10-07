using System.Runtime.InteropServices;

namespace Microsoft.VSFolders.Console
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ConsoleFontInfo
    {
        [FieldOffset(0)]
        public uint NFont;

        [FieldOffset(4)]
        public Coord FontSize;
    }
}