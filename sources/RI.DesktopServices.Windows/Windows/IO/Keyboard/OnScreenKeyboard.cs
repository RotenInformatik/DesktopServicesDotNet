using System;
using System.Diagnostics;
using System.Runtime.InteropServices;




namespace RI.Framework.IO.Keyboard
{
    /// <summary>
    ///     Provides access to the Windows on-screen keyboard.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    /// TODO: Test if this still works
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




        #region Static Properties/Indexer

        /// <summary>
        ///     Gets whether the on-screen keyboard is available or not.
        /// </summary>
        /// <value>
        ///     true if the on-screen keyboard is available, false otherwise.
        /// </value>
        public static bool IsAvailable => OnScreenKeyboard.ExecutablePath != null;

        private static FilePath ExecutablePath
        {
            get
            {
                FilePath filePath = new FilePath(OnScreenKeyboard.TabTipExecutablePath);

                foreach (Environment.SpecialFolder folder in OnScreenKeyboard.TabTipFolders)
                {
                    FilePath path = new DirectoryPath(Environment.GetFolderPath(folder)).AppendFile(filePath);

                    if (path.Exists)
                    {
                        return path;
                    }
                }

                return null;
            }
        }

        #endregion




        #region Static Methods

        /// <summary>
        ///     Deactivates the on-screen keyboard.
        /// </summary>
        public static void Hide ()
        {
            if (!OnScreenKeyboard.IsAvailable)
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
        ///     Activates the on-screen keyboard.
        /// </summary>
        /// <returns>
        ///     true if the on-screen keyboard could be activated, false otherwise.
        /// </returns>
        public static bool Show ()
        {
            if (!OnScreenKeyboard.IsAvailable)
            {
                return false;
            }

            FilePath executablePath = OnScreenKeyboard.ExecutablePath;

            ProcessStartInfo startInfo = new ProcessStartInfo(executablePath);
            startInfo.ErrorDialog = false;
            startInfo.UseShellExecute = true;

            using (Process process = Process.Start(startInfo))
            {
                return process != null;
            }
        }

        [DllImport("user32.dll", SetLastError = false, CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow (string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr SendMessage (IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        #endregion
    }
}
