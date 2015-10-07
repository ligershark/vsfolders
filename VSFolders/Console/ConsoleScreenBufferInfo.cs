using System.Runtime.InteropServices;

namespace Microsoft.VSFolders.Console
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ConsoleScreenBufferInfo
    {
        [FieldOffset(0)]
        public Coord Size;
        [FieldOffset(4)]
        public Coord CursorPosition;
        [FieldOffset(8)]
        public ushort Attributes;
        [FieldOffset(10)]
        public SmallRect Window;
        [FieldOffset(18)]
        public Coord MaximumWindowSize;
    }
}