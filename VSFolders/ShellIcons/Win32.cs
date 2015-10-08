// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Win32.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   Win32.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.ShellIcons
{
    using System;
    using System.Runtime.InteropServices;

    public static class Win32
    {
        public const uint SHGFI_ICON = 0x000000100;

        public const uint SHGFI_DISPLAYNAME = 0x000000200;

        public const uint SHGFI_TYPENAME = 0x000000400;

        public const uint SHGFI_ATTRIBUTES = 0x000000800;

        public const uint SHGFI_ICONLOCATION = 0x000001000;

        public const uint SHGFI_EXETYPE = 0x000002000;

        public const uint SHGFI_SYSICONINDEX = 0x000004000;

        public const uint SHGFI_LINKOVERLAY = 0x000008000;

        public const uint SHGFI_SELECTED = 0x000010000;

        public const uint SHGFI_ATTR_SPECIFIED = 0x000020000;

        public const uint SHGFI_LARGEICON = 0x000000000;

        public const uint SHGFI_SMALLICON = 0x000000001;

        public const uint SHGFI_OPENICON = 0x000000002;

        public const uint SHGFI_SHELLICONSIZE = 0x000000004;

        public const uint SHGFI_PIDL = 0x000000008;

        public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;

        public const uint SHGFI_ADDOVERLAYS = 0x000000020;

        public const uint SHGFI_OVERLAYINDEX = 0x000000040;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;

            public IntPtr iIcon;

            public uint dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)] public string szTypeName;
        };
    }
}