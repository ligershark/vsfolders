using System.Runtime.InteropServices;

namespace Microsoft.VSFolders.Console
{
    [StructLayout(LayoutKind.Explicit)]
    public struct SmallRect
    {
        [FieldOffset(0)]
        public ushort Left;
        [FieldOffset(2)]
        public ushort Top;
        [FieldOffset(4)]
        public ushort Right;
        [FieldOffset(6)]
        public ushort Bottom;
    }
}