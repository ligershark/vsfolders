using System.Runtime.InteropServices;

namespace Microsoft.VSFolders
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Coord
    {
        [FieldOffset(0)]
        public ushort X;
        [FieldOffset(2)]
        public ushort Y;
    }
}