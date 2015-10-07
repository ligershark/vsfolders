using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VSFolders.Models;
using Microsoft.VSFolders.Settings;
using Microsoft.VSFolders.ShellIcons;
using MessageBox = System.Windows.MessageBox;
using Panel = System.Windows.Forms.Panel;
using Timer = System.Threading.Timer;

namespace Microsoft.VSFolders.Console
{
    /// <summary>
    ///     Interaction logic for CommandControl.xaml
    /// </summary>
    public partial class ConsoleControl : IDisposable
    {
        private readonly Panel _panel;
        private static Process _process;
        private static IntPtr _windowHandle;
        private static ConsoleControl _currentInstance;

        public ConsoleControl()
        {
            InitializeComponent();
            _currentInstance = this;
            _panel = new Panel();
            WindowsFormsHost1.Child = _panel;
            Loaded += OnLoaded;
            WindowsFormsHost1.Child.GotFocus += WindowsFormsHost1OnGotFocus;
            WindowsFormsHost1.Child.SizeChanged += WindowsFormsHost1OnSizeChanged;
        }

        private void SettingsOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "DefaultConsole" && VSFoldersPackage.Settings != null && VSFoldersPackage.Settings.Consoles != null)
            {
                ConsoleEnv.SelectedItem = VSFoldersPackage.Settings.Consoles.FirstOrDefault(x => x.Key == VSFoldersPackage.Settings.DefaultConsole);
            }
        }

        private void WindowsFormsHost1OnSizeChanged(object sender, EventArgs eventArgs)
        {
            _isAutomation = true;
            WindowsFormsHost1.Child.Focus();
            _isAutomation = false;
        }

        private static bool _isAutomation;

        private void WindowsFormsHost1OnGotFocus(object sender, EventArgs eventArgs)
        {
            if (_process == null)
            {
                return;
            }

            _process.Refresh();

            if (_process.HasExited)
            {
                return;
            }


            if (!_isAutomation && Mouse.DirectlyOver != WindowsFormsHost1)
            {
                return;
            }

            WindowsFormsHost1.Child.GotFocus -= WindowsFormsHost1OnGotFocus;
            ResizeEmbeddedApp();
            NativeMethods.SetActiveWindow(_windowHandle);
            WindowsFormsHost1.Child.GotFocus += WindowsFormsHost1OnGotFocus;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ConsoleEnv.SelectedValue = _currentConsole;
            InitConsole(true);
            VSFoldersPackage.Settings.PropertyChanged += SettingsOnPropertyChanged;
        }

        private static void ProcessOnExited(object sender, EventArgs eventArgs)
        {
            var proc = (Process)sender;
            proc.EnableRaisingEvents = false;
            proc.Exited -= ProcessOnExited;

            _windowHandle = IntPtr.Zero;
            _process = null;

            if (_currentInstance.CheckAccess())
            {
                InitConsole();
            }
            else
            {
                _currentInstance.Dispatcher.Invoke(() => InitConsole());
            }
        }

        public Size GetScreenSize(string text, FontFamily fontFamily, double fontSize, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch)
        {
            fontFamily = fontFamily ?? new TextBlock().FontFamily;
            fontSize = fontSize > 0 ? fontSize : new TextBlock().FontSize;
            var typeface = new Typeface(fontFamily, fontStyle, fontWeight, fontStretch);
            var ft = new FormattedText(text ?? string.Empty, CultureInfo.CurrentCulture, FlowDirection, typeface, fontSize, Brushes.Black);
            return new Size(ft.Width, ft.Height);
        }

        private static void InitConsole(bool reuse = false)
        {
            try
            {
                _consoleReady = false;

                if (!reuse || _process == null)
                {
                    var psi = new ProcessStartInfo(_currentConsole.Executable)
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = false
                    };

                    if (!string.IsNullOrWhiteSpace(_currentConsole.Arguments))
                    {
                        psi.Arguments = _currentConsole.Arguments;
                    }

                    _process = Process.Start(psi);
                    _process.EnableRaisingEvents = true;
                    _process.Exited += ProcessOnExited;
                    _process.Refresh();
                }


                if (!reuse || _windowHandle == IntPtr.Zero)
                {
                    var waits = 0;
                    const int waitTimeout = 400;

                    while (waits++ < waitTimeout && _windowHandle == IntPtr.Zero && !_process.HasExited && _process.MainWindowHandle == IntPtr.Zero)
                    {
                        _process.Refresh();
                        Thread.Sleep(1);
                    }

                    if (waits >= waitTimeout)
                    {
                        KillConsole(_currentConsole.ExitCommand, _currentConsole.ExitDelay, true);
                        return;
                    }

                    _windowHandle = _process.MainWindowHandle;
                }

                NativeMethods.SetParent(_windowHandle, _currentInstance._panel.Handle);

                // resize embedded application & refresh
                ResizeEmbeddedApp();
                ResizeEmbeddedApp();
                _consoleReady = true;
            }
            catch (Exception ex)
            {
                if (_process.HasExited)
                {
                    return;
                }

                MessageBox.Show(ex.ToString());
            }
        }

        private const int SwpNoZOrder = 0x0004;
        private const int SwpNoActivate = 0x0010;
        private const int SwpFrameChanged = 0x0020;
        private const int GwlStyle = -16;
        private const int WsCaption = 0x00C00000;
        private const int WsThickframe = 0x00040000;
        private static Timer _timer;

        private static void KillConsole(string exitCommand, int exitDelay, bool modeSwitch)
        {
            _consoleReady = false;
            _isAutomation = true;
            SetWindowPos(true);
            _isAutomation = false;

            if (_process != null)
            {
                if (!modeSwitch)
                {
                    _process.EnableRaisingEvents = false;
                    _process.Exited -= ProcessOnExited;
                }

                _process.Refresh();
                _timer = new Timer(Callback);
                _timer.Change(TimeSpan.FromMilliseconds(exitDelay < 100 ? 100 : exitDelay), TimeSpan.FromMilliseconds(1000));

                if (!_process.HasExited && !string.IsNullOrWhiteSpace(exitCommand))
                {
                    SendKeys.SendWait(exitCommand);
                    SendKeys.SendWait("{ENTER}");
                }
            }
        }

        private static void Callback(object state)
        {
            if (_timer != null)
            {
                var timer = _timer;

                if (timer != null)
                {
                    _timer = null;
                    timer.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(0));
                    timer.Dispose();
                }
            }
            else
            {
                return;
            }

            if (_process != null)
            {
                _process.Refresh();

                if (!_process.HasExited)
                {
                    _windowHandle = IntPtr.Zero;
                    _process.Kill();
                    _process = null;
                }
            }
        }

        public static void CleanupForVisualStudioShutdown()
        {
            if (_process != null)
            {
                KillConsole(_currentConsole.ExitCommand, _currentConsole.ExitDelay, false);

                while (_process != null && !_process.HasExited)
                {
                    _process.Refresh();
                    Thread.Sleep(1);
                }
            }
        }

        protected virtual void Dispose(bool isDisposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ConsoleControl()
        {
            Dispose(false);
        }

        private static void ResizeEmbeddedApp()
        {
            if (_process == null)
            {
                return;
            }

            _process.Refresh();

            if (_process.HasExited)
            {
                return;
            }

            ResizeConsoleWindow();

            //NativeMethods.SetParent(_windowHandle, IntPtr.Zero);
            //NativeMethods.SetParent(_windowHandle, _panel.Handle);
            SetWindowPos();
        }

        private static void SetWindowPos(bool activate = false)
        {
            // remove control box
            var style = NativeMethods.GetWindowLong(_windowHandle, GwlStyle);
            style = ((style | 0x00200000) & ~0x00040000); //& ~WsCaption;
            NativeMethods.SetWindowLong(_windowHandle, GwlStyle, style);

            var borderFix = _process.ProcessName.Equals("cmd", StringComparison.OrdinalIgnoreCase) || _process.ProcessName.Equals("powershell", StringComparison.OrdinalIgnoreCase);
            var leftFix = borderFix ? -8 : -3;
            var rightFix = borderFix ? 23 : 6;
            var bottomFix = borderFix ? 48 : 35;
            var topFix = borderFix ? -33 : -29;

            NativeMethods.SetWindowPos(_windowHandle, IntPtr.Zero, leftFix, topFix, (int)_currentInstance.WindowsFormsHost1.ActualWidth + rightFix, (int)_currentInstance.WindowsFormsHost1.ActualHeight + bottomFix, SwpNoZOrder | SwpFrameChanged | 0x0100 | (activate ? 0 : SwpNoActivate));

            var rgn = NativeMethods.CreateRectRgn(leftFix, topFix, (int)_currentInstance.WindowsFormsHost1.ActualWidth + rightFix, (int)_currentInstance.WindowsFormsHost1.ActualHeight + bottomFix);

            if (rgn != IntPtr.Zero)
            {
                NativeMethods.SetWindowRgn(_windowHandle, rgn, true);
                NativeMethods.ShowWindow(_windowHandle, 1);
            }

            NativeMethods.InvalidateRect(_windowHandle, IntPtr.Zero, true);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            ResizeEmbeddedApp();
        }

        private void TrackFoldersWindowSelection(object sender, RoutedEventArgs e)
        {
            var ctrl = ((FoldersControl)VSFoldersPackage.DemandFoldersWindow().Content);
            var arg = ctrl.ViewModel.Path;

            if (string.IsNullOrWhiteSpace(arg))
            {
                return;
            }

            if (File.Exists(arg))
            {
                arg = Path.GetDirectoryName(arg);
            }

            if (!Directory.Exists(arg))
            {
                return;
            }

            _process.StartInfo.WorkingDirectory = arg;
            var path = arg.Replace("\\", _currentConsole.Separator);
            _isAutomation = true;
            WindowsFormsHost1.Child.Focus();
            SetWindowPos(true);
            _isAutomation = false;
            SendKeys.Send("cd \"" + path + "\"");
            SendKeys.Send("{ENTER}");
        }

        private static void ResizeConsoleWindow()
        {
            NativeMethods.AttachConsole((uint)_process.Id);
            ConsoleScreenBufferInfo coninfo;
            var handle = NativeMethods.GetStdHandle(-11);
            NativeMethods.GetConsoleScreenBufferInfo(handle, out coninfo);
            ConsoleFontInfo font;
            NativeMethods.GetCurrentConsoleFont(handle, true, out font);
            font.FontSize = NativeMethods.GetConsoleFontSize(handle, font.NFont);
            coninfo.Size.X = (ushort)((_currentInstance.ActualWidth / font.FontSize.X) + 2);
            coninfo.Size.Y = (ushort)((_currentInstance.ActualHeight / font.FontSize.Y) + 2);
            NativeMethods.SetConsoleScreenBufferSize(handle, coninfo.Size);
            NativeMethods.FreeConsole();
        }

        private static ConsoleDefinition _currentConsole = GetDefaultConsole();
        private static bool _consoleReady;

        private static ConsoleDefinition GetDefaultConsole()
        {
            ConsoleDefinition console;
            if (VSFoldersPackage.Settings.Consoles.TryGetValue(VSFoldersPackage.Settings.DefaultConsole ?? "", out console) && console != null)
            {
                return console;
            }

            var firstConsole = VSFoldersPackage.Settings.Consoles.FirstOrDefault();

            if (firstConsole.Key != null)
            {
                VSFoldersPackage.Settings.DefaultConsole = firstConsole.Key;
                return firstConsole.Value;
            }

            return null;
        }

        private void ConsoleEnv_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ConsoleEnv.SelectedItem == null)
            {
                return;
            }

            var consolePair = (KeyValuePair<string, ConsoleDefinition>)ConsoleEnv.SelectedItem;

            if (consolePair.Value == null || consolePair.Key == VSFoldersPackage.Settings.DefaultConsole)
            {
                return;
            }

            VSFoldersPackage.Settings.DefaultConsole = consolePair.Key;
            var exitCommand = _currentConsole.ExitCommand;
            var exitDelay = _currentConsole.ExitDelay;
            _currentConsole = consolePair.Value;
            KillConsole(exitCommand, exitDelay, true);
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            VSFoldersPackage.ShowDialogPage<ConsoleSettingsPage>();
        }

        public void OpenAt(FileData fileData)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                if (fileData == null)
                {
                    return;
                }

                while (!_consoleReady || _currentConsole == null || _windowHandle == IntPtr.Zero)
                {
                    Thread.Sleep(100);
                }

                Thread.Sleep(500);
                string arg = fileData.FullPath;
                if (File.Exists(fileData.FullPath))
                {
                    arg = Path.GetDirectoryName(fileData.FullPath);
                }

                if (!Directory.Exists(arg))
                {
                    return;
                }

                var path = arg.Replace("\\", _currentConsole.Separator);
                Dispatcher.Invoke(() =>
                {
                    _isAutomation = true;
                    WindowsFormsHost1.Child.Focus();
                    SetWindowPos(true);
                    _isAutomation = false;
                    SendKeys.SendWait("cd \"" + path + "\"");
                    SendKeys.SendWait("{ENTER}");
                });
            });
        }
    }
}