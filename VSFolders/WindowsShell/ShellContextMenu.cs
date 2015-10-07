using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.VSFolders.WindowsShell
{
    public class ShellContextMenu : NativeWindow, IDisposable
    {
        private IntPtr[] _arrPIDLs;
        private IContextMenu _oContextMenu;
        private IContextMenu2 _oContextMenu2;
        private IContextMenu3 _oContextMenu3;
        private IShellFolder _oDesktopFolder;
        private IShellFolder _oParentFolder;
        private string _strParentFolder;
        private static readonly int CbInvokeCommand = Marshal.SizeOf(typeof(CMINVOKECOMMANDINFOEX));
        private const uint CmdFirst = 1;
        private const uint CmdLast = 0x7530;
        private static Guid _iidIContextMenu = new Guid("{000214e4-0000-0000-c000-000000000046}");
        private static Guid _iidIContextMenu2 = new Guid("{000214f4-0000-0000-c000-000000000046}");
        private static Guid _iidIContextMenu3 = new Guid("{bcfce0a0-ec17-11d0-8d10-00a0c90f2719}");
        private static Guid _iidIShellFolder = new Guid("{000214E6-0000-0000-C000-000000000046}");
        private const int MaxPath = 260;
        private const int Win32Ok = 0;

        public ShellContextMenu()
        {
            CreateHandle(new CreateParams());
        }

        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CreatePopupMenu();
        
        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool DestroyMenu(IntPtr hMenu);
        
        public void Dispose()
        {
            ReleaseAll();
        }

        protected void FreePIDLs(IntPtr[] pidls)
        {
            if (null != pidls)
            {
                for (int i = 0; i < pidls.Length; i++)
                {
                    if (pidls[i] != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(pidls[i]);
                        pidls[i] = IntPtr.Zero;
                    }
                }
            }
        }

        private bool GetContextMenuInterfaces(IShellFolder oParentFolder, IntPtr[] pidls, out IntPtr ctxMenuPtr)
        {
            var success = oParentFolder.GetUIObjectOf(IntPtr.Zero, (uint)pidls.Length, pidls, ref _iidIContextMenu, IntPtr.Zero, out ctxMenuPtr);
            
            if (Win32Ok == success)
            {
                _oContextMenu = (IContextMenu)Marshal.GetTypedObjectForIUnknown(ctxMenuPtr, typeof(IContextMenu));
                return true;
            }

            ctxMenuPtr = IntPtr.Zero;
            _oContextMenu = null;
            return false;
        }

        private IShellFolder GetDesktopFolder()
        {
            if (null == _oDesktopFolder)
            {
                IntPtr hndl;
                var num = SHGetDesktopFolder(out hndl);
                
                if (Win32Ok != num)
                {
                    throw new ShellContextMenuException("Failed to get the desktop shell folder");
                }
                
                _oDesktopFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(hndl, typeof(IShellFolder));
            }

            return _oDesktopFolder;
        }

        private IShellFolder GetParentFolder(string folderName)
        {
            if (null == _oParentFolder)
            {
                var desktopFolder = GetDesktopFolder();
                
                if (null == desktopFolder)
                {
                    return null;
                }

                IntPtr zero;
                uint pchEaten = 0;
                SFGAO pdwAttributes = 0;
                var num2 = desktopFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, folderName, ref pchEaten, out zero, ref pdwAttributes);
                
                if (Win32Ok != num2)
                {
                    return null;
                }

                var ptr = Marshal.AllocCoTaskMem(0x20c);
                Marshal.WriteInt32(ptr, 0, 0);
                _oDesktopFolder.GetDisplayNameOf(zero, SHGNO.FORPARSING, ptr);
                StringBuilder pszBuf = new StringBuilder(MaxPath);
                StrRetToBuf(ptr, zero, pszBuf, MaxPath);
                Marshal.FreeCoTaskMem(ptr);
                _strParentFolder = pszBuf.ToString();
                IntPtr ppv;
                num2 = desktopFolder.BindToObject(zero, IntPtr.Zero, ref _iidIShellFolder, out ppv);
                Marshal.FreeCoTaskMem(zero);
                
                if (Win32Ok != num2)
                {
                    return null;
                }

                _oParentFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(ppv, typeof(IShellFolder));
            }
            return _oParentFolder;
        }

        protected IntPtr[] GetPIDLs(FileSystemInfo[] infos)
        {
            if ((infos == null) || (0 == infos.Length))
            {
                return null;
            }
            
            var parentFolder = infos[0] is DirectoryInfo ? GetParentFolder(((DirectoryInfo)infos[0]).Parent.FullName) : GetParentFolder(((FileInfo)infos[0]).DirectoryName);
            
            if (null == parentFolder)
            {
                return null;
            }
            
            var arrPIDLs = new IntPtr[infos.Length];
            var index = 0;

            foreach (var info in infos)
            {
                uint pchEaten = 0;
                SFGAO pdwAttributes = 0;
                IntPtr hndl;

                var num3 = parentFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, info.Name, ref pchEaten, out hndl, ref pdwAttributes);
                
                if (Win32Ok != num3)
                {
                    FreePIDLs(arrPIDLs);
                    return null;
                }

                arrPIDLs[index] = hndl;
                index++;
            }
            return arrPIDLs;
        }

        private static void InvokeCommand(IContextMenu oContextMenu, uint nCmd, string strFolder, Point pointInvoke)
        {
            CMINVOKECOMMANDINFOEX info = new CMINVOKECOMMANDINFOEX
            {
                cbSize = CbInvokeCommand,
                lpVerb = (IntPtr)(nCmd - 1),
                lpDirectory = strFolder,
                lpVerbW = (IntPtr)(nCmd - 1),
                lpDirectoryW = strFolder,
                fMask = ((CMIC.PTINVOKE | CMIC.UNICODE) | (((Control.ModifierKeys & Keys.Control) != Keys.None) ? CMIC.CONTROL_DOWN : 0)) | (((Control.ModifierKeys & Keys.Shift) != Keys.None) ? CMIC.SHIFT_DOWN : 0),
                ptInvoke = new POINT(pointInvoke.X, pointInvoke.Y),
                nShow = SW.NORMAL
            };
            oContextMenu.InvokeCommand(ref info);
        }

        private void ReleaseAll()
        {
            if (null != _oContextMenu)
            {
                Marshal.ReleaseComObject(_oContextMenu);
                _oContextMenu = null;
            }

            if (null != _oContextMenu2)
            {
                Marshal.ReleaseComObject(_oContextMenu2);
                _oContextMenu2 = null;
            }

            if (null != _oContextMenu3)
            {
                Marshal.ReleaseComObject(_oContextMenu3);
                _oContextMenu3 = null;
            }

            if (null != _oDesktopFolder)
            {
                Marshal.ReleaseComObject(_oDesktopFolder);
                _oDesktopFolder = null;
            }

            if (null != _oParentFolder)
            {
                Marshal.ReleaseComObject(_oParentFolder);
                _oParentFolder = null;
            }

            if (null != _arrPIDLs)
            {
                FreePIDLs(_arrPIDLs);
                _arrPIDLs = null;
            }
        }

        [DllImport("shell32.dll")]
        private static extern int SHGetDesktopFolder(out IntPtr ppshf);

        private void ShowContextMenu(Point pointScreen)
        {
            var hndl = IntPtr.Zero;
            var ctxMenuPtr = IntPtr.Zero;
            var ppv = IntPtr.Zero;
            var ptr4 = IntPtr.Zero;

            try
            {
                if (null == _arrPIDLs)
                {
                    ReleaseAll();
                }
                else if (!GetContextMenuInterfaces(_oParentFolder, _arrPIDLs, out ctxMenuPtr))
                {
                    ReleaseAll();
                }
                else
                {
                    hndl = CreatePopupMenu();

                    try
                    {
                        _oContextMenu.QueryContextMenu(hndl, 0, CmdFirst, CmdLast, CMF.EXPLORE | (((Control.ModifierKeys & Keys.Shift) != Keys.None) ? CMF.EXTENDEDVERBS : CMF.NORMAL));
                    }
                    catch (Exception)
                    {
                        Trace.WriteLine(string.Format("_oContextMenu={0}\npMenu={1}\n_arrPIDLs={2}\n_oParentFolder={3}", new object[] { _oContextMenu, hndl, string.Join(",", _arrPIDLs), _oParentFolder }));
                        throw;
                    }

                    Marshal.QueryInterface(ctxMenuPtr, ref _iidIContextMenu2, out ppv);
                    Marshal.QueryInterface(ctxMenuPtr, ref _iidIContextMenu3, out ptr4);
                    _oContextMenu2 = (IContextMenu2)Marshal.GetTypedObjectForIUnknown(ppv, typeof(IContextMenu2));
                    _oContextMenu3 = (IContextMenu3)Marshal.GetTypedObjectForIUnknown(ptr4, typeof(IContextMenu3));
                    var nCmd = TrackPopupMenuEx(hndl, TPM.HORIZONTAL | TPM.RETURNCMD, pointScreen.X, pointScreen.Y, Handle, IntPtr.Zero);
                    DestroyMenu(hndl);
                    hndl = IntPtr.Zero;

                    if (nCmd != 0)
                    {
                        InvokeCommand(_oContextMenu, nCmd, _strParentFolder, pointScreen);
                    }
                }
            }
            finally
            {
                if (hndl != IntPtr.Zero)
                {
                    DestroyMenu(hndl);
                }
             
                if (ctxMenuPtr != IntPtr.Zero)
                {
                    Marshal.Release(ctxMenuPtr);
                }
                
                if (ppv != IntPtr.Zero)
                {
                    Marshal.Release(ppv);
                }
                
                if (ptr4 != IntPtr.Zero)
                {
                    Marshal.Release(ptr4);
                }
                
                ReleaseAll();
            }
        }

        public void ShowContextMenu(FileSystemInfo[] files, Point pointScreen)
        {
            ReleaseAll();
            _arrPIDLs = GetPIDLs(files);
            ShowContextMenu(pointScreen);
        }

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int StrRetToBuf(IntPtr pstr, IntPtr pidl, StringBuilder pszBuf, int cchBuf);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern uint TrackPopupMenuEx(IntPtr hmenu, TPM flags, int x, int y, IntPtr hwnd, IntPtr lptpm);

        protected override void WndProc(ref Message m)
        {
            if ((((_oContextMenu != null) && (m.Msg == 0x11f)) && ((ShellHelper.HiWord(m.WParam) & 0x800) == 0)) && ((ShellHelper.HiWord(m.WParam) & 0x10) == 0))
            {
                if (ShellHelper.LoWord(m.WParam) == 0x7531)
                {
                }
            }
            if ((((_oContextMenu2 == null) || (((m.Msg != 0x117) && (m.Msg != 0x2c)) && (m.Msg != 0x2b))) || (_oContextMenu2.HandleMenuMsg((uint)m.Msg, m.WParam, m.LParam) != 0)) && (((_oContextMenu3 == null) || (m.Msg != 0x120)) || (_oContextMenu3.HandleMenuMsg2((uint)m.Msg, m.WParam, m.LParam, IntPtr.Zero) != 0)))
            {
                base.WndProc(ref m);
            }
        }

        [Flags]
        private enum CMF : uint
        {
            EXPLORE = 4,
            EXTENDEDVERBS = 0x100,
            NORMAL = 0
        }

        [Flags]
        private enum CMIC : uint
        {
            CONTROL_DOWN = 0x40000000,
            PTINVOKE = 0x20000000,
            SHIFT_DOWN = 0x10000000,
            UNICODE = 0x4000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CMINVOKECOMMANDINFOEX
        {
            public int cbSize;
            public CMIC fMask;
            public IntPtr hwnd;
            public IntPtr lpVerb;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpDirectory;
            public ShellContextMenu.SW nShow;
            public int dwHotKey;
            public IntPtr hIcon;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpTitle;
            public IntPtr lpVerbW;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpParametersW;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpDirectoryW;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpTitleW;
            public POINT ptInvoke;
        }

        [Flags]
        private enum GCS : uint
        {
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214e4-0000-0000-c000-000000000046")]
        private interface IContextMenu
        {
            [PreserveSig]
            int QueryContextMenu(IntPtr hmenu, uint iMenu, uint idCmdFirst, uint idCmdLast, ShellContextMenu.CMF uFlags);
            [PreserveSig]
            int InvokeCommand(ref CMINVOKECOMMANDINFOEX info);
            [PreserveSig]
            int GetCommandString(uint idcmd, GCS uflags, uint reserved, [MarshalAs(UnmanagedType.LPArray)] byte[] commandstring, int cch);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214f4-0000-0000-c000-000000000046")]
        private interface IContextMenu2
        {
            [PreserveSig]
            int QueryContextMenu(IntPtr hmenu, uint iMenu, uint idCmdFirst, uint idCmdLast, CMF uFlags);
            [PreserveSig]
            int InvokeCommand(ref CMINVOKECOMMANDINFOEX info);
            [PreserveSig]
            int GetCommandString(uint idcmd, GCS uflags, uint reserved, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder commandstring, int cch);
            [PreserveSig]
            int HandleMenuMsg(uint uMsg, IntPtr wParam, IntPtr lParam);
        }

        [ComImport, Guid("bcfce0a0-ec17-11d0-8d10-00a0c90f2719"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IContextMenu3
        {
            [PreserveSig]
            int QueryContextMenu(IntPtr hmenu, uint iMenu, uint idCmdFirst, uint idCmdLast, CMF uFlags);
            [PreserveSig]
            int InvokeCommand(ref CMINVOKECOMMANDINFOEX info);
            [PreserveSig]
            int GetCommandString(uint idcmd, GCS uflags, uint reserved, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder commandstring, int cch);
            [PreserveSig]
            int HandleMenuMsg(uint uMsg, IntPtr wParam, IntPtr lParam);
            [PreserveSig]
            int HandleMenuMsg2(uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr plResult);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214E6-0000-0000-C000-000000000046")]
        private interface IShellFolder
        {
            [PreserveSig]
            int ParseDisplayName(IntPtr hwnd, IntPtr pbc, [MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, ref uint pchEaten, out IntPtr ppidl, ref ShellContextMenu.SFGAO pdwAttributes);
            [PreserveSig]
            int EnumObjects(IntPtr hwnd, SHCONTF grfFlags, out IntPtr enumIDList);
            [PreserveSig]
            int BindToObject(IntPtr pidl, IntPtr pbc, ref Guid riid, out IntPtr ppv);
            [PreserveSig]
            int BindToStorage(IntPtr pidl, IntPtr pbc, ref Guid riid, out IntPtr ppv);
            [PreserveSig]
            int CompareIDs(IntPtr lParam, IntPtr pidl1, IntPtr pidl2);
            [PreserveSig]
            int CreateViewObject(IntPtr hwndOwner, Guid riid, out IntPtr ppv);
            [PreserveSig]
            int GetAttributesOf(uint cidl, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl, ref SFGAO rgfInOut);
            [PreserveSig]
            int GetUIObjectOf(IntPtr hwndOwner, uint cidl, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl, ref Guid riid, IntPtr rgfReserved, out IntPtr ppv);
            [PreserveSig]
            int GetDisplayNameOf(IntPtr pidl, SHGNO uFlags, IntPtr lpName);
            [PreserveSig]
            int SetNameOf(IntPtr hwnd, IntPtr pidl, [MarshalAs(UnmanagedType.LPWStr)] string pszName, SHGNO uFlags, out IntPtr ppidlOut);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct POINT
        {
            public int x;
            public int y;
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [Flags]
        private enum SFGAO : uint
        {
        }

        [Flags]
        private enum SHCONTF
        {
        }

        [Flags]
        private enum SHGNO
        {
            FORPARSING = 0x8000
        }

        [Flags]
        private enum SW
        {
            NORMAL = 1,
        }

        [Flags]
        private enum TPM : uint
        {
            HORIZONTAL = 0,
            RETURNCMD = 0x100
        }
    }
}
