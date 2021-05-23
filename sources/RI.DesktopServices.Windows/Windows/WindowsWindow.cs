using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;




namespace RI.Framework.Windows
{
    /// <summary>
    ///     Provides utilities for working with native windows.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Native windows are represented using their window handle (HWND).
    ///     </para>
    /// </remarks>
    public static class WindowsWindow
    {
        #region Constants

        private const uint SmtoAbortifhung = 0x0002;

        private const uint SmtoNormal = 0x000;

        private const uint SwHide = 0;

        private const uint SwpNoactivate = 0x0010;

        private const uint SwpNomove = 0x0002;

        private const uint SwpNosize = 0x0001;

        private const uint SwpNozorder = 0x0004;

        private const uint SwShow = 5;

        private const uint SwShowmaximized = 3;

        private const uint SwShowminimized = 2;

        private const uint SwShownormal = 1;

        private const uint WmGettext = 0x000D;

        private const uint WmGettextlength = 0x000E;

        #endregion




        #region Static Constructor/Destructor

        static WindowsWindow ()
        {
            WindowsWindow.FindChildWindowsSyncRoot = new object();
            WindowsWindow.FindWindowsSyncRoot = new object();
        }

        #endregion




        #region Static Fields

        private static readonly IntPtr HwndBottom = new IntPtr(1);

        #endregion




        #region Static Properties/Indexer

        private static object FindChildWindowsSyncRoot { get; set; }

        private static List<IntPtr> FindChildWindowsWindows { get; set; }

        private static object FindWindowsSyncRoot { get; set; }

        private static List<IntPtr> FindWindowsWindows { get; set; }

        #endregion




        #region Static Methods

        /// <summary>
        ///     Enables or disables a window.
        /// </summary>
        /// <param name="hWnd"> The window to enable/disable. </param>
        /// <param name="enable"> true if the window should be enabled, false if it should be disabled. </param>
        /// <remarks>
        ///     <para>
        ///         Nothing happens if <paramref name="hWnd" /> is <see cref="IntPtr.Zero" />.
        ///     </para>
        /// </remarks>
        public static void EnableWindow (IntPtr hWnd, bool enable)
        {
            if (hWnd == IntPtr.Zero)
            {
                return;
            }

            WindowsWindow.EnableWindowInternal(hWnd, enable);
        }

        /// <summary>
        ///     Gets all child windows of a window.
        /// </summary>
        /// <param name="hWnd"> The window for which all child windows are to be found. </param>
        /// <returns>
        ///     The array with window handles to all the child windows of the specified window.
        ///     An empty array is returned if no child windows are available or <paramref name="hWnd" /> is
        ///     <see cref="IntPtr.Zero" />.
        /// </returns>
        public static IntPtr[] FindChildWindows (IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
            {
                return new IntPtr[0];
            }

            lock (WindowsWindow.FindChildWindowsSyncRoot)
            {
                if (WindowsWindow.FindChildWindowsWindows == null)
                {
                    WindowsWindow.FindChildWindowsWindows = new List<IntPtr>();
                }

                WindowsWindow.FindChildWindowsWindows.Clear();
                WindowsWindow.EnumChildWindows(hWnd, WindowsWindow.FindChildWindowsProc, IntPtr.Zero);
                return WindowsWindow.FindChildWindowsWindows.ToArray();
            }
        }

        /// <summary>
        ///     Gets all top-level windows.
        /// </summary>
        /// <returns>
        ///     The array with window handles to all the top-level windows.
        ///     An empty array is returned if no top-level windows are available.
        /// </returns>
        public static IntPtr[] FindTopWindows ()
        {
            lock (WindowsWindow.FindWindowsSyncRoot)
            {
                if (WindowsWindow.FindWindowsWindows == null)
                {
                    WindowsWindow.FindWindowsWindows = new List<IntPtr>();
                }

                WindowsWindow.FindWindowsWindows.Clear();
                WindowsWindow.EnumWindows(WindowsWindow.FindWindowsProc, IntPtr.Zero);
                return WindowsWindow.FindWindowsWindows.ToArray();
            }
        }

        /// <summary>
        ///     Gets the date and time when the last user input was made.
        /// </summary>
        /// <returns>
        ///     The date and time when the last user input was made.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         This method is not necessarily precise and the time since the last user input can be affected by various system
        ///         components or settings, other applications, or connected devices.
        ///         Therefore, only use the returned value for non-critical things which do not need to be precise.
        ///     </note>
        /// </remarks>
        public static DateTime GetLastInput ()
        {
            LASTINPUTINFO info = new LASTINPUTINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);
            WindowsWindow.GetLastInputInfo(ref info);

            double idleTicks = (Environment.TickCount - info.dwTime) * (-1.0);
            DateTime inputTimestamp = DateTime.Now.AddMilliseconds(idleTicks);
            return inputTimestamp;
        }

        /// <summary>
        ///     Gets the process associated with a window.
        /// </summary>
        /// <param name="hWnd"> The window. </param>
        /// <returns>
        ///     The process to which the specified window is associated or null if no process can be found or
        ///     <paramref name="hWnd" /> is <see cref="IntPtr.Zero" />.
        /// </returns>
        public static Process GetProcess (IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
            {
                return null;
            }

            int processId = 0;
            WindowsWindow.GetWindowThreadProcessId(hWnd, ref processId);

            Process[] processes = Process.GetProcesses();

            Process process = (from p in processes
                               where p.Id == processId
                               select p).FirstOrDefault();

            return process;
        }

        /// <summary>
        ///     Gets all windows associated with a process.
        /// </summary>
        /// <param name="process"> The process. </param>
        /// <returns>
        ///     The array with all window handles to all the windows of the specified process.
        ///     An empty array is returned if the process does not have any windows.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="process" /> is null. </exception>
        public static IntPtr[] GetWindows (Process process)
        {
            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            IntPtr[] allWindows = WindowsWindow.FindTopWindows();

            IntPtr[] foundWindows = (from w in allWindows
                                     where WindowsWindow.GetProcess(w)
                                                        .Id == process.Id
                                     select w).ToArray();

            return foundWindows;
        }

        /// <summary>
        ///     Gets the title of a window.
        /// </summary>
        /// <param name="hWnd"> The window. </param>
        /// <returns>
        ///     The title of the window or null if the title cannot be retrieved or <paramref name="hWnd" /> is
        ///     <see cref="IntPtr.Zero" />.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         Retrieving a window title can fail in cases the window is blocked and does not react to window messages within
        ///         one second.
        ///     </para>
        /// </remarks>
        public static string GetWindowTitle (IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
            {
                return null;
            }

            int length = -1;

            WindowsWindow.SendMessageTimeout1(hWnd, WindowsWindow.WmGettextlength, IntPtr.Zero, IntPtr.Zero,
                                              WindowsWindow.SmtoAbortifhung | WindowsWindow.SmtoNormal, 1000,
                                              ref length);

            if (length == -1)
            {
                return null;
            }

            if (length == 0)
            {
                return string.Empty;
            }

            if (length > 10240)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder(length + 1);

            IntPtr temp = new IntPtr();

            WindowsWindow.SendMessageTimeout2(hWnd, WindowsWindow.WmGettext, new IntPtr(sb.Capacity), sb,
                                              WindowsWindow.SmtoAbortifhung | WindowsWindow.SmtoNormal, 1000, ref temp);

            if (temp.ToInt64() == -1)
            {
                return null;
            }

            if (temp.ToInt64() == 0)
            {
                return string.Empty;
            }

            string title = sb.ToString();
            return title;
        }

        /// <summary>
        ///     Hides a window.
        /// </summary>
        /// <param name="hWnd"> The window to hide. </param>
        /// <remarks>
        ///     <para>
        ///         Nothing happens if <paramref name="hWnd" /> is <see cref="IntPtr.Zero" />.
        ///     </para>
        /// </remarks>
        public static void HideWindow (IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
            {
                return;
            }

            WindowsWindow.ShowWindow(hWnd, (int)WindowsWindow.SwHide);
        }

        /// <summary>
        ///     Maximizes a window.
        /// </summary>
        /// <param name="hWnd"> The window to maximize. </param>
        /// <remarks>
        ///     <para>
        ///         Nothing happens if <paramref name="hWnd" /> is <see cref="IntPtr.Zero" />.
        ///     </para>
        /// </remarks>
        public static void MaximizeWindow (IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
            {
                return;
            }

            WindowsWindow.ShowWindow(hWnd, (int)WindowsWindow.SwShowmaximized);
        }

        /// <summary>
        ///     Minimizes a window.
        /// </summary>
        /// <param name="hWnd"> The window to minimize. </param>
        /// <remarks>
        ///     <para>
        ///         Nothing happens if <paramref name="hWnd" /> is <see cref="IntPtr.Zero" />.
        ///     </para>
        /// </remarks>
        public static void MinimizeWindow (IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
            {
                return;
            }

            WindowsWindow.ShowWindow(hWnd, (int)WindowsWindow.SwShowminimized);
        }

        /// <summary>
        ///     Moves a window to the background (behind other windows).
        /// </summary>
        /// <param name="hWnd"> The window to move to the background. </param>
        /// <remarks>
        ///     <para>
        ///         Nothing happens if <paramref name="hWnd" /> is <see cref="IntPtr.Zero" />.
        ///     </para>
        /// </remarks>
        public static void MoveWindowToBackground (IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
            {
                return;
            }

            WindowsWindow.SetWindowPos(hWnd, WindowsWindow.HwndBottom, 0, 0, 0, 0,
                                       WindowsWindow.SwpNosize | WindowsWindow.SwpNomove | WindowsWindow.SwpNoactivate);
        }

        /// <summary>
        ///     Moves a window to the foreground (ontop of other windows).
        /// </summary>
        /// <param name="hWnd"> The window to move to the foreground. </param>
        /// <remarks>
        ///     <para>
        ///         Nothing happens if <paramref name="hWnd" /> is <see cref="IntPtr.Zero" />.
        ///     </para>
        /// </remarks>
        public static void MoveWindowToForeground (IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
            {
                return;
            }

            WindowsWindow.SetForegroundWindow(hWnd);
            WindowsWindow.BringWindowToTop(hWnd);
            WindowsWindow.SetActiveWindow(hWnd);
        }

        /// <summary>
        ///     Moves a window to the primary screen.
        /// </summary>
        /// <param name="hWnd"> The window to move. </param>
        /// <remarks>
        ///     <para>
        ///         Nothing happens if <paramref name="hWnd" /> is <see cref="IntPtr.Zero" />.
        ///     </para>
        /// </remarks>
        public static void MoveWindowToPrimaryScreen (IntPtr hWnd)
        {
            WindowsWindow.MoveWindowToScreen(hWnd, null);
        }

        /// <summary>
        ///     Moves a window to a screen.
        /// </summary>
        /// <param name="hWnd"> The window to move. </param>
        /// <param name="screenIndex"> The screen index or -1 to move to the primary screen. </param>
        /// <remarks>
        ///     <para>
        ///         Nothing happens if <paramref name="hWnd" /> is <see cref="IntPtr.Zero" />.
        ///     </para>
        /// </remarks>
        public static void MoveWindowToScreen (IntPtr hWnd, int screenIndex)
        {
            if (screenIndex < -1)
            {
                screenIndex = 0;
            }

            if (screenIndex >= Screen.AllScreens.Length)
            {
                screenIndex = Screen.AllScreens.Length - 1;
            }

            WindowsWindow.MoveWindowToScreen(hWnd, screenIndex == -1 ? null : Screen.AllScreens[screenIndex]);
        }

        /// <summary>
        ///     Moves a window to a screen.
        /// </summary>
        /// <param name="hWnd"> The window to move. </param>
        /// <param name="screen"> The screen or null to move to the primary screen. </param>
        /// <remarks>
        ///     <para>
        ///         Nothing happens if <paramref name="hWnd" /> is <see cref="IntPtr.Zero" />.
        ///     </para>
        /// </remarks>
        public static void MoveWindowToScreen (IntPtr hWnd, Screen screen)
        {
            if (hWnd == IntPtr.Zero)
            {
                return;
            }

            screen = screen ?? Screen.PrimaryScreen;

            WINDOWINFO info = new WINDOWINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);
            WindowsWindow.GetWindowInfo(hWnd, ref info);

            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.Length = (uint)Marshal.SizeOf(placement);
            WindowsWindow.GetWindowPlacement(hWnd, ref placement);

            bool isMaximized = placement.ShowCmd == WindowsWindow.SwShowmaximized;
            bool isMinimized = placement.ShowCmd == WindowsWindow.SwShowminimized;
            bool isNormal = placement.ShowCmd == WindowsWindow.SwShownormal;

            int width = info.rcWindow.Right - info.rcWindow.Left;
            int height = info.rcWindow.Bottom - info.rcWindow.Top;

            if (isMaximized || isMinimized)
            {
                WindowsWindow.ShowWindow(hWnd, (int)WindowsWindow.SwShownormal);
            }

            WindowsWindow.SetWindowPos(hWnd, IntPtr.Zero, screen.WorkingArea.Left, screen.WorkingArea.Top, 0, 0,
                                       WindowsWindow.SwpNozorder | WindowsWindow.SwpNosize);

            if (isNormal)
            {
                WindowsWindow.SetWindowPos(hWnd, IntPtr.Zero,
                                           screen.WorkingArea.Left + ((screen.WorkingArea.Width - width) / 2),
                                           screen.WorkingArea.Top + ((screen.WorkingArea.Height - height) / 2), 0, 0,
                                           WindowsWindow.SwpNozorder | WindowsWindow.SwpNosize);
            }
            else if (isMaximized)
            {
                WindowsWindow.ShowWindow(hWnd, (int)WindowsWindow.SwShowmaximized);
            }
            else if (isMinimized)
            {
                WindowsWindow.ShowWindow(hWnd, (int)WindowsWindow.SwShowminimized);
            }
        }

        /// <summary>
        ///     Sets a window to its normal position and size.
        /// </summary>
        /// <param name="hWnd"> The window to set to its normal position and size. </param>
        /// <remarks>
        ///     <para>
        ///         Nothing happens if <paramref name="hWnd" /> is <see cref="IntPtr.Zero" />.
        ///     </para>
        /// </remarks>
        public static void NormalizeWindow (IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
            {
                return;
            }

            WindowsWindow.ShowWindow(hWnd, (int)WindowsWindow.SwShownormal);
        }

        /// <summary>
        ///     Moves a window to a new position and size.
        /// </summary>
        /// <param name="hWnd"> The window to move to a new position and size. </param>
        /// <param name="x"> The new x position of the window (top-left of the window). </param>
        /// <param name="y"> The new y position of the window (top-left of the window). </param>
        /// <param name="width"> The new width of the window. </param>
        /// <param name="height"> The new height of the window. </param>
        /// <remarks>
        ///     <para>
        ///         Nothing happens if <paramref name="hWnd" /> is <see cref="IntPtr.Zero" />.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="width" /> or <paramref name="height" /> is less than
        ///     zero.
        /// </exception>
        public static void RelocateWindow (IntPtr hWnd, int x, int y, int width, int height)
        {
            if (width < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }

            if (height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            if (hWnd == IntPtr.Zero)
            {
                return;
            }

            WindowsWindow.MoveWindow(hWnd, x, y, width, height, true);
        }

        /// <summary>
        ///     Shows a window.
        /// </summary>
        /// <param name="hWnd"> The window to show. </param>
        /// <remarks>
        ///     <para>
        ///         Nothing happens if <paramref name="hWnd" /> is <see cref="IntPtr.Zero" />.
        ///     </para>
        /// </remarks>
        public static void ShowWindow (IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
            {
                return;
            }

            WindowsWindow.ShowWindow(hWnd, (int)WindowsWindow.SwShow);
        }

        [DllImport("user32.dll", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool BringWindowToTop (IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = false, EntryPoint = "EnableWindow")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnableWindowInternal (IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool enable);

        [DllImport("user32.dll", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows (IntPtr hWnd, EnumChildWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows (EnumWindowsProc lpEnumFunc, IntPtr lParam);

        private static bool FindChildWindowsProc (IntPtr hWnd, ref IntPtr lParam)
        {
            WindowsWindow.FindChildWindowsWindows.Add(hWnd);
            return true;
        }

        private static bool FindWindowsProc (IntPtr hWnd, ref IntPtr lParam)
        {
            WindowsWindow.FindWindowsWindows.Add(hWnd);
            return true;
        }

        [DllImport("user32.dll", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetLastInputInfo (ref LASTINPUTINFO plii);

        [DllImport("user32.dll", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowInfo (IntPtr hwnd, ref WINDOWINFO pwi);

        [DllImport("user32.dll", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowPlacement (IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", SetLastError = false)]
        private static extern uint GetWindowThreadProcessId (IntPtr hWnd, ref int lpdwProcessId);

        [DllImport("User32.dll", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool MoveWindow (IntPtr handle, int x, int y, int width, int height,
                                               [MarshalAs(UnmanagedType.Bool)] bool redraw);

        [DllImport("user32.dll", SetLastError = false, EntryPoint = "SendMessageTimeout")]
        private static extern IntPtr SendMessageTimeout1 (IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam,
                                                          uint fuFlags, uint uTimeout, ref int lpdwResult);

        [DllImport("user32.dll", SetLastError = false, EntryPoint = "SendMessageTimeout", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessageTimeout2 (IntPtr hWnd, uint msg, IntPtr wParam, StringBuilder lParam,
                                                          uint fuFlags, uint uTimeout, ref IntPtr lpdwResult);

        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr SetActiveWindow (IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow (IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos (IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy,
                                                 uint uFlags);

        [DllImport("user32.dll", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow (IntPtr hWnd, int nCmdShow);

        #endregion




        #region Type: EnumChildWindowsProc

        private delegate bool EnumChildWindowsProc (IntPtr hWnd, ref IntPtr lParam);

        #endregion




        #region Type: EnumWindowsProc

        private delegate bool EnumWindowsProc (IntPtr hWnd, ref IntPtr lParam);

        #endregion




        #region Type: LASTINPUTINFO

        [StructLayout(LayoutKind.Sequential)]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private struct LASTINPUTINFO
        {
            public uint cbSize;

            public uint dwTime;
        }

        #endregion




        #region Type: POINT

        [StructLayout(LayoutKind.Sequential)]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private struct POINT
        {
            public int X;

            public int Y;
        }

        #endregion




        #region Type: RECT

        [StructLayout(LayoutKind.Sequential)]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private struct RECT
        {
            public int Left;

            public int Top;

            public int Right;

            public int Bottom;
        }

        #endregion




        #region Type: WINDOWINFO

        [StructLayout(LayoutKind.Sequential)]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private struct WINDOWINFO
        {
            public uint cbSize;

            public RECT rcWindow;

            public RECT rcClient;

            public uint dwStyle;

            public uint dwExStyle;

            public uint dwWindowStatus;

            public uint cxWindowBorders;

            public uint cyWindowBorders;

            public ushort atomWindowType;

            public ushort wCreatorVersion;
        }

        #endregion




        #region Type: WINDOWPLACEMENT

        [StructLayout(LayoutKind.Sequential)]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private struct WINDOWPLACEMENT
        {
            public uint Length;

            public uint Flags;

            public uint ShowCmd;

            public POINT MinPosition;

            public POINT MaxPosition;

            public RECT NormalPosition;
        }

        #endregion
    }
}
