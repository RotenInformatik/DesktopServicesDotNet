using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;




namespace RI.DesktopServices.Windows.IO.Keyboard
{
    /// <summary>
    ///     Provides access to the Windows on-screen keyboard.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class OnScreenKeyboard
    {
        #region Constants

        private const int ScClose = 0xF060;

        private const string TabTipClassName = "IPTIP_Main_Window";

        private const string TabTipExecutablePath = @"Microsoft Shared\Ink\TabTip.exe";

        private const string TabTipWindowName = "";

        private const int WmSyscommand = 0x0112;

        #endregion




        #region Static Fields

        private static readonly Environment.SpecialFolder[] TabTipFolders =
        {
            Environment.SpecialFolder.CommonProgramFiles,
            Environment.SpecialFolder.CommonProgramFilesX86,
        };

        #endregion




        #region Static Methods

        /// <summary>
        ///     Deactivates the on-screen keyboard.
        /// </summary>
        public static void Hide ()
        {
            if (!OnScreenKeyboard.IsAvailable())
            {
                return;
            }

            IntPtr windowHandle =
                OnScreenKeyboard.FindWindow(OnScreenKeyboard.TabTipClassName, OnScreenKeyboard.TabTipWindowName);

            if (windowHandle != IntPtr.Zero)
            {
                OnScreenKeyboard.SendMessage(windowHandle, OnScreenKeyboard.WmSyscommand,
                                             new IntPtr(OnScreenKeyboard.ScClose), IntPtr.Zero);
            }
        }

        /// <summary>
        ///     Gets whether the on-screen keyboard is available or not.
        /// </summary>
        /// <returns>
        ///     true if the on-screen keyboard is available, false otherwise.
        /// </returns>
        public static bool IsAvailable () => OnScreenKeyboard.GetExecutablePath() != null;

        /// <summary>
        ///     Activates the on-screen keyboard.
        /// </summary>
        /// <returns>
        ///     true if the on-screen keyboard could be activated, false otherwise.
        /// </returns>
        public static bool Show ()
        {
            string executablePath = OnScreenKeyboard.GetExecutablePath();

            if (executablePath == null)
            {
                Trace.TraceWarning($"On-screen keyboard not available");
                return false;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo(executablePath);
            startInfo.ErrorDialog = false;
            startInfo.UseShellExecute = true;

            using (Process process = Process.Start(startInfo))
            {
                if (process == null)
                {
                    Trace.TraceWarning($"On-screen keyboard failed to start: {executablePath}");
                }

                return process != null;
            }
        }

        [DllImport("user32.dll", SetLastError = false, CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow (string lpClassName, string lpWindowName);

        private static string GetExecutablePath ()
        {
            foreach (Environment.SpecialFolder folder in OnScreenKeyboard.TabTipFolders)
            {
                string path = Path.Join(Environment.GetFolderPath(folder), OnScreenKeyboard.TabTipExecutablePath);

                if (File.Exists(path))
                {
                    return path;
                }
            }

            return null;
        }

        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr SendMessage (IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        #endregion
    }
}
