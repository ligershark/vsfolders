using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Microsoft.VSFolders.ShellIcons
{
    public static class IconUtil
    {
        private static readonly ConcurrentDictionary<string, BitmapSource> IconCache = new ConcurrentDictionary<string, BitmapSource>(); 
        private static readonly Lazy<BitmapSource> FolderIcon = new Lazy<BitmapSource>(GetIconForFolder);

        public static BitmapSource GetIcon(string path)
        {
            if (File.Exists(path))
            {
                return GetIconForFileName(path);
            }

            if (Directory.Exists(path))
            {
                return FolderIcon.Value;
            }

            return null;
        }

        public static BitmapSource RefreshIcon(string path)
        {
            if (File.Exists(path))
            {
                return RefreshIconForFileName(path);
            }

            if (Directory.Exists(path))
            {
                return FolderIcon.Value;
            }

            return null;
        }

        private static BitmapSource GetIconForFileNameInternal(string fileName)
        {
            return Imaging.CreateBitmapSourceFromHIcon(Icon.ExtractAssociatedIcon(fileName).Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        private static BitmapSource GetIconForFileName(string fileName)
        {
            return IconCache.GetOrAdd(fileName, GetIconForFileNameInternal);
        }

        private static BitmapSource RefreshIconForFileName(string fileName)
        {
            return IconCache.AddOrUpdate(fileName, GetIconForFileNameInternal, (x, e) => GetIconForFileNameInternal(x));
        }

        private static BitmapSource GetIconForFolder()
        {
            var fileInfo = new Win32.SHFILEINFO();
            var hresult = NativeMethods.SHGetFileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), (uint) FileAttributes.Directory, ref fileInfo, (uint) Marshal.SizeOf(typeof (Win32.SHFILEINFO)), Win32.SHGFI_ICON | Win32.SHGFI_ADDOVERLAYS | Win32.SHGFI_SMALLICON | Win32.SHGFI_USEFILEATTRIBUTES);

            if (hresult == IntPtr.Zero)
            {
                throw new ArgumentException("Couldn't get icon for folders");
            }

            var bitmap = Imaging.CreateBitmapSourceFromHIcon(fileInfo.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            NativeMethods.DestroyIcon(fileInfo.hIcon);
            return bitmap;
        }
    }
}