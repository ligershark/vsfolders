// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IconUtil.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   IconUtil.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.ShellIcons
{
    using System;
    using System.Collections.Concurrent;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;

    public static class IconUtil
    {
        private static readonly ConcurrentDictionary<string, BitmapSource> IconCache =
            new ConcurrentDictionary<string, BitmapSource>(StringComparer.OrdinalIgnoreCase);

        private static readonly Lazy<BitmapSource> FolderIcon = new Lazy<BitmapSource>(IconUtil.GetIconForFolder);

        public static BitmapSource GetIcon(string path)
        {
            if (File.Exists(path))
            {
                return IconUtil.GetIconForFileName(path);
            }

            if (Directory.Exists(path))
            {
                return IconUtil.FolderIcon.Value;
            }

            return null;
        }

        public static BitmapSource RefreshIcon(string path)
        {
            if (File.Exists(path))
            {
                return IconUtil.GetIconForFileName(path);
            }

            if (Directory.Exists(path))
            {
                return IconUtil.FolderIcon.Value;
            }

            return null;
        }

        private static BitmapSource GetIconForFileNameInternal(string fileName)
        {
            return Imaging.CreateBitmapSourceFromHIcon(
                Icon.ExtractAssociatedIcon(fileName).Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private static BitmapSource GetIconForFileName(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            if (!string.IsNullOrEmpty(extension))
            {
                return IconUtil.IconCache.GetOrAdd(
                extension,
                x => IconUtil.GetIconForFileNameInternal(fileName));
            }

            return IconUtil.GetIconForFileNameInternal(fileName);
        }

        private static BitmapSource GetIconForFolder()
        {
            Win32.SHFILEINFO fileInfo = new Win32.SHFILEINFO();
            IntPtr hresult =
                NativeMethods.SHGetFileInfo(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    (uint)FileAttributes.Directory,
                    ref fileInfo,
                    (uint)Marshal.SizeOf(typeof(Win32.SHFILEINFO)),
                    Win32.SHGFI_ICON | Win32.SHGFI_ADDOVERLAYS | Win32.SHGFI_SMALLICON | Win32.SHGFI_USEFILEATTRIBUTES);

            if (hresult == IntPtr.Zero)
            {
                throw new ArgumentException("Couldn't get icon for folders");
            }

            BitmapSource bitmap = Imaging.CreateBitmapSourceFromHIcon(
                fileInfo.hIcon,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            NativeMethods.DestroyIcon(fileInfo.hIcon);
            return bitmap;
        }
    }
}