// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   NativeMethods.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.ShellIcons
{
    using System;
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32")]
        public static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

        [DllImport("user32")]
        public static extern bool SetWindowPos(
            IntPtr hWnd, 
            IntPtr hWndInsertAfter, 
            int x, 
            int y, 
            int cx, 
            int cy, 
            int uFlags);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern bool AttachConsole(uint processId);

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        public static extern Coord GetConsoleFontSize(IntPtr hConsoleOutput, uint nFont);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(int stdHandle);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(
            string pszPath, 
            uint dwFileAttributes, 
            ref Win32.SHFILEINFO psfi, 
            uint cbFileInfo, 
            uint uFlags);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetConsoleScreenBufferSize(IntPtr hConsoleOutput, Coord dwSize);

        [DllImport("user32")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        [DllImport("Gdi32.dll", SetLastError = true)]
        public static extern IntPtr CreateRectRgn(int left, int top, int right, int bottom);

        [DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr windowHandle, int cmd);

        [DllImport("user32.dll")]
        public static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);
    }
}