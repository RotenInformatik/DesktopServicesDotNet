using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

using RI.Framework.Windows.Shell;




namespace RI.Framework.Windows
{
    /// <summary>
    ///     Provides utilities for working with Windows applications.
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         When waiting for exit before a new process is started, e.g. on restarting the current application, the wait for
    ///         exit and the restart are delegated into a batch file, executed in its own process.
    ///         Therefore, the action will silently fail on errors.
    ///     </note>
    /// </remarks>
    public static class WindowsApplication
    {
        #region Constants

        private const string WaitForExitScript = ":loop\r\n" + "tasklist | find \"{0}\" >nul\r\n" +
                                                 "if not errorlevel 1 (\r\n" + "timeout /T 1 /NOBREAK >nul\r\n" +
                                                 "goto :loop\r\n" + ")\r\n";

        #endregion




        #region Static Methods

        /// <summary>
        ///     Restarts the current application including the current command line arguments.
        /// </summary>
        /// <param name="waitForExitBeforeStart">
        ///     Specifies whether it should be waited for the current process to exit before the
        ///     application is restarted.
        /// </param>
        /// <remarks>
        ///     <note type="note">
        ///         See the remarks of <see cref="WindowsApplication" /> for more important information about windows application
        ///         actions provided by <see cref="WindowsApplication" />.
        ///     </note>
        /// </remarks>
        public static void RestartCurrent (bool waitForExitBeforeStart) =>
            WindowsApplication.RestartCurrentInternal(waitForExitBeforeStart, CommandLine.FromCurrentProcess());

        /// <summary>
        ///     Restarts the current application with the specified command line arguments.
        /// </summary>
        /// <param name="waitForExitBeforeStart">
        ///     Specifies whether it should be waited for the current process to exit before the
        ///     application is restarted.
        /// </param>
        /// <param name="commandLineArguments"> The used command line arguments or null if no command line arguments are used. </param>
        /// <remarks>
        ///     <note type="note">
        ///         See the remarks of <see cref="WindowsApplication" /> for more important information about windows application
        ///         actions provided by <see cref="WindowsApplication" />.
        ///     </note>
        /// </remarks>
        public static void RestartCurrent (bool waitForExitBeforeStart, string commandLineArguments) =>
            WindowsApplication.RestartCurrentInternal(waitForExitBeforeStart,
                                                      CommandLine.Parse(commandLineArguments, false));

        /// <summary>
        ///     Restarts the current application with the specified command line arguments.
        /// </summary>
        /// <param name="waitForExitBeforeStart">
        ///     Specifies whether it should be waited for the current process to exit before the
        ///     application is restarted.
        /// </param>
        /// <param name="commandLineArguments"> The used command line arguments or null if no command line arguments are used. </param>
        /// <remarks>
        ///     <note type="note">
        ///         See the remarks of <see cref="WindowsApplication" /> for more important information about windows application
        ///         actions provided by <see cref="WindowsApplication" />.
        ///     </note>
        /// </remarks>
        public static void RestartCurrent (bool waitForExitBeforeStart, CommandLine commandLineArguments) =>
            WindowsApplication.RestartCurrentInternal(waitForExitBeforeStart, commandLineArguments);

        private static void RestartCurrentInternal (bool waitForExitBeforeStart, CommandLine commandLine)
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            FilePath assemblyFile = entryAssembly?.GetFile();

            if (assemblyFile == null)
            {
                throw new InvalidOperationException("The current application seems to be of a non-restartable kind.");
            }

            Process currentProcess = Process.GetCurrentProcess();

            if (waitForExitBeforeStart)
            {
                string script = WindowsApplication.WaitForExitScript.NormalizeLineBreaks()
                                                  .Trim();

                script = string.Format(CultureInfo.InvariantCulture, script, currentProcess.Id);
                script = script + Environment.NewLine + "\"" + assemblyFile + "\"";

                if (commandLine != null)
                {
                    script = script + " " + commandLine.Build();
                }

                Process newProcess = WindowsShell.ExecuteBatchCommands(script, null);
                newProcess?.Dispose();
            }
            else
            {
                ProcessStartInfo startInfo = currentProcess.StartInfo;
                startInfo.FileName = assemblyFile;

                if (commandLine != null)
                {
                    startInfo.Arguments = commandLine.Build();
                }

                Process newProcess = Process.Start(startInfo);
                newProcess?.Dispose();
            }
        }

        #endregion
    }
}
