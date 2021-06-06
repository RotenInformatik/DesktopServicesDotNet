using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using RI.DesktopServices.Windows.Interop;




namespace RI.DesktopServices.Windows.Shell
{
    /// <summary>
    ///     Provides utilities for controlling the current Windows session.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class WindowsSession
    {
        #region Constants

        private const uint FlagsForceIfHung = 0x10;

        private const uint FlagsLogOff = 0x00;

        private const uint FlagsPowerOff = 0x08;

        private const uint FlagsReboot = 0x02;

        private const uint ReasonFlagUserDefined = 0x40000000;

        private const uint ReasonMajorApplication = 0x00040000;

        private const uint ReasonMinorOther = 0x00000000;

        #endregion




        #region Static Methods

        /// <summary>
        ///     Logs-off the current user.
        /// </summary>
        /// <param name="force">
        ///     Specifies whether the logoff is forced (true), terminating all busy applications, or busy
        ///     applications prevent from logging-off (false).
        /// </param>
        /// <exception cref="InvalidOperationException"> The necessary privilege adjustments failed. </exception>
        public static void Logoff (bool force)
        {
            Trace.TraceInformation($"Windows session: Logoff (force: {force})");

            WindowsSession.AdjustPrivileges();

            if (force)
            {
                WindowsSession.ExitWindowsEx(WindowsSession.FlagsLogOff | WindowsSession.FlagsForceIfHung,
                                             WindowsSession.ReasonMajorApplication | WindowsSession.ReasonMinorOther |
                                             WindowsSession.ReasonFlagUserDefined);
            }
            else
            {
                WindowsSession.ExitWindowsEx(WindowsSession.FlagsLogOff,
                                             WindowsSession.ReasonMajorApplication | WindowsSession.ReasonMinorOther |
                                             WindowsSession.ReasonFlagUserDefined);
            }
        }

        /// <summary>
        ///     Restarts the machine.
        /// </summary>
        /// <param name="force">
        ///     Specifies whether the restart is forced (true), terminating all busy applications, or busy
        ///     applications prevent from restarting (false).
        /// </param>
        /// <exception cref="InvalidOperationException"> The necessary privilege adjustments failed. </exception>
        public static void Restart (bool force)
        {
            Trace.TraceInformation($"Windows session: Restart (force: {force})");

            WindowsSession.AdjustPrivileges();

            if (force)
            {
                WindowsSession.ExitWindowsEx(WindowsSession.FlagsReboot | WindowsSession.FlagsForceIfHung,
                                             WindowsSession.ReasonMajorApplication | WindowsSession.ReasonMinorOther |
                                             WindowsSession.ReasonFlagUserDefined);
            }
            else
            {
                WindowsSession.ExitWindowsEx(WindowsSession.FlagsReboot,
                                             WindowsSession.ReasonMajorApplication | WindowsSession.ReasonMinorOther |
                                             WindowsSession.ReasonFlagUserDefined);
            }
        }

        /// <summary>
        ///     Shuts-down the machine.
        /// </summary>
        /// <param name="force">
        ///     Specifies whether the shutdown is forced (true), terminating all busy applications, or busy
        ///     applications prevent from shutting-down (false).
        /// </param>
        /// <exception cref="InvalidOperationException"> The necessary privilege adjustments failed. </exception>
        public static void Shutdown (bool force)
        {
            Trace.TraceInformation($"Windows session: Shutdown (force: {force})");

            WindowsSession.AdjustPrivileges();

            if (force)
            {
                WindowsSession.ExitWindowsEx(WindowsSession.FlagsPowerOff | WindowsSession.FlagsForceIfHung,
                                             WindowsSession.ReasonMajorApplication | WindowsSession.ReasonMinorOther |
                                             WindowsSession.ReasonFlagUserDefined);
            }
            else
            {
                WindowsSession.ExitWindowsEx(WindowsSession.FlagsPowerOff,
                                             WindowsSession.ReasonMajorApplication | WindowsSession.ReasonMinorOther |
                                             WindowsSession.ReasonFlagUserDefined);
            }
        }

        private static void AdjustPrivileges ()
        {
            Privileges.EnablePrivilege(SecurityEntity.SeShutdownName);
        }

        [DllImport("user32.dll", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ExitWindowsEx (uint uFlags, uint dwReason);

        #endregion
    }
}
