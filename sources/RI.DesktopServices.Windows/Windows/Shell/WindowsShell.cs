using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;




namespace RI.DesktopServices.Windows.Shell
{
    /// <summary>
    ///     Provides utilities for working with the Windows shell environment.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class WindowsShell
    {
        #region Constants

        private const string BatchFileExtension = ".bat";

        private const string CommandPromptArguments = "/k \"cd /d {0}\"";

        private const string CommandPromptExecutable = "cmd.exe";

        private const string ElevatedVerb = "runas";

        private const string ExplorerExecutable = "explorer.exe";

        private const string SystemInfoExecutable = "msinfo32.exe";

        private const string TaskManagerExecutable = "taskmgr.exe";

        #endregion




        #region Static Methods

        /// <summary>
        ///     Opens a command prompt.
        /// </summary>
        /// <returns>
        ///     true if the command prompt could be opened, false otherwise.
        /// </returns>
        public static bool OpenCommandPrompt () => WindowsShell.OpenCommandPrompt(null, false);

        /// <summary>
        ///     Opens a command prompt.
        /// </summary>
        /// <param name="workingDirectory"> The used working directory. Can be null to use the current directory. </param>
        /// <param name="elevated"> Specifies whether the command prompt should be opened with elevated privileges. </param>
        /// <returns>
        ///     true if the command prompt could be opened, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         Environment variables will be resolved for <paramref name="workingDirectory" />.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentException"> <paramref name="workingDirectory" /> is an empty string. </exception>
        public static bool OpenCommandPrompt (string workingDirectory, bool elevated)
        {
            if (workingDirectory != null)
            {
                if (string.IsNullOrWhiteSpace(workingDirectory))
                {
                    throw new ArgumentException("The string is empty.", nameof(workingDirectory));
                }
            }

            workingDirectory ??= Environment.CurrentDirectory;

            string directory = Environment.ExpandEnvironmentVariables(workingDirectory);

            string arguments = string.Format(CultureInfo.InvariantCulture, WindowsShell.CommandPromptArguments,
                                             directory);

            ProcessStartInfo startInfo = new ProcessStartInfo(WindowsShell.CommandPromptExecutable, arguments);
            startInfo.CreateNoWindow = false;
            startInfo.ErrorDialog = false;
            startInfo.UseShellExecute = true;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.WorkingDirectory = directory;

            if (elevated)
            {
                startInfo.Verb = WindowsShell.ElevatedVerb;
            }

            try
            {
                Process.Start(startInfo);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Opens the Windows Explorer.
        /// </summary>
        /// <returns>
        ///     true if the Windows Explorer could be opened, false otherwise.
        /// </returns>
        public static bool OpenExplorer () => WindowsShell.OpenExplorer(ProcessWindowStyle.Normal, false);

        /// <summary>
        ///     Opens the Windows Explorer.
        /// </summary>
        /// <param name="windowStyle"> The window style of the opened Windows Explorer window. </param>
        /// <param name="elevated"> Specifies whether the Windows Explorer should be opened with elevated privileges. </param>
        /// <returns>
        ///     true if the Windows Explorer could be opened, false otherwise.
        /// </returns>
        public static bool OpenExplorer (ProcessWindowStyle windowStyle, bool elevated)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(WindowsShell.ExplorerExecutable);

            startInfo.CreateNoWindow = false;
            startInfo.ErrorDialog = false;
            startInfo.UseShellExecute = true;
            startInfo.WindowStyle = windowStyle;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;

            if (elevated)
            {
                startInfo.Verb = WindowsShell.ElevatedVerb;
            }

            try
            {
                Process.Start(startInfo);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Opens a file with its associated program.
        /// </summary>
        /// <param name="filePath"> The file path to open. </param>
        /// <returns>
        ///     true if the file could be opened, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         Environment variables will be resolved for <paramref name="filePath" />.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="filePath" /> is an empty string. </exception>
        public static bool OpenFile (string filePath) =>
            WindowsShell.OpenFile(filePath, ProcessWindowStyle.Normal, false);

        /// <summary>
        ///     Opens a file with its associated program.
        /// </summary>
        /// <param name="filePath"> The file path to open. </param>
        /// <param name="windowStyle"> The window style of the opened program window. </param>
        /// <param name="elevated"> Specifies whether the file should be opened with elevated privileges. </param>
        /// <returns>
        ///     true if the file could be opened, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         Environment variables will be resolved for <paramref name="filePath" />.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="filePath" /> is an empty string. </exception>
        public static bool OpenFile (string filePath, ProcessWindowStyle windowStyle, bool elevated)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("The string is empty.", nameof(filePath));
            }

            string file = Environment.ExpandEnvironmentVariables(filePath);
            string directory = Environment.ExpandEnvironmentVariables(Path.GetDirectoryName(filePath) ?? string.Empty);

            if (string.IsNullOrWhiteSpace(directory))
            {
                directory = Environment.CurrentDirectory;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo(file);

            startInfo.CreateNoWindow = false;
            startInfo.ErrorDialog = false;
            startInfo.UseShellExecute = true;
            startInfo.WindowStyle = windowStyle;
            startInfo.WorkingDirectory = directory;

            if (elevated)
            {
                startInfo.Verb = WindowsShell.ElevatedVerb;
            }

            try
            {
                Process.Start(startInfo);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Opens a folder in Windows Explorer.
        /// </summary>
        /// <param name="folderPath"> The folder path to open. </param>
        /// <returns>
        ///     true if the folder could be opened, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         Environment variables will be resolved for <paramref name="folderPath" />.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="folderPath" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="folderPath" /> is an empty string. </exception>
        public static bool OpenFolder (string folderPath) =>
            WindowsShell.OpenFolder(folderPath, ProcessWindowStyle.Normal, false);

        /// <summary>
        ///     Opens a folder in Windows Explorer.
        /// </summary>
        /// <param name="folderPath"> The folder path to open. </param>
        /// <param name="windowStyle"> The window style of the opened Windows Explorer window. </param>
        /// <param name="elevated"> Specifies whether the folder should be opened with elevated privileges. </param>
        /// <returns>
        ///     true if the folder could be opened, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         Environment variables will be resolved for <paramref name="folderPath" />.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="folderPath" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="folderPath" /> is an empty string. </exception>
        public static bool OpenFolder (string folderPath, ProcessWindowStyle windowStyle, bool elevated)
        {
            if (folderPath == null)
            {
                throw new ArgumentNullException(nameof(folderPath));
            }

            if (string.IsNullOrWhiteSpace(folderPath))
            {
                throw new ArgumentException("The string is empty.", nameof(folderPath));
            }

            string directory = Environment.ExpandEnvironmentVariables(folderPath);

            ProcessStartInfo startInfo = new ProcessStartInfo(directory);

            startInfo.CreateNoWindow = false;
            startInfo.ErrorDialog = false;
            startInfo.UseShellExecute = true;
            startInfo.WindowStyle = windowStyle;
            startInfo.WorkingDirectory = directory;

            if (elevated)
            {
                startInfo.Verb = WindowsShell.ElevatedVerb;
            }

            try
            {
                Process.Start(startInfo);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Opens the System Info.
        /// </summary>
        /// <returns>
        ///     true if the System Info could be opened, false otherwise.
        /// </returns>
        public static bool OpenSystemInfo () => WindowsShell.OpenSystemInfo(ProcessWindowStyle.Normal, false);

        /// <summary>
        ///     Opens the System Info.
        /// </summary>
        /// <param name="windowStyle"> The window style of the opened System Info window. </param>
        /// <param name="elevated"> Specifies whether the System Info should be opened with elevated privileges. </param>
        /// <returns>
        ///     true if the System Info could be opened, false otherwise.
        /// </returns>
        public static bool OpenSystemInfo (ProcessWindowStyle windowStyle, bool elevated)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(WindowsShell.SystemInfoExecutable);

            startInfo.CreateNoWindow = false;
            startInfo.ErrorDialog = false;
            startInfo.UseShellExecute = true;
            startInfo.WindowStyle = windowStyle;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;

            if (elevated)
            {
                startInfo.Verb = WindowsShell.ElevatedVerb;
            }

            try
            {
                Process.Start(startInfo);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Opens the Task Manager.
        /// </summary>
        /// <returns>
        ///     true if the Task Manager could be opened, false otherwise.
        /// </returns>
        public static bool OpenTaskManager () => WindowsShell.OpenTaskManager(ProcessWindowStyle.Normal, false);

        /// <summary>
        ///     Opens the Task Manager.
        /// </summary>
        /// <param name="windowStyle"> The window style of the opened Task Manager window. </param>
        /// <param name="elevated"> Specifies whether the Task Manager should be opened with elevated privileges. </param>
        /// <returns>
        ///     true if the Task Manager could be opened, false otherwise.
        /// </returns>
        public static bool OpenTaskManager (ProcessWindowStyle windowStyle, bool elevated)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(WindowsShell.TaskManagerExecutable);

            startInfo.CreateNoWindow = false;
            startInfo.ErrorDialog = false;
            startInfo.UseShellExecute = true;
            startInfo.WindowStyle = windowStyle;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;

            if (elevated)
            {
                startInfo.Verb = WindowsShell.ElevatedVerb;
            }

            try
            {
                Process.Start(startInfo);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Opens an URL.
        /// </summary>
        /// <param name="url"> The URL to open. </param>
        /// <returns>
        ///     true if the URL could be opened, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="url" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="url" /> is an empty string. </exception>
        /// <exception cref="UriFormatException"> <paramref name="url" /> is not a valid URI. </exception>
        public static bool OpenUrl (string url) => WindowsShell.OpenUrl(url, ProcessWindowStyle.Normal, false);

        /// <summary>
        ///     Opens an URL.
        /// </summary>
        /// <param name="url"> The URL to open. </param>
        /// <param name="windowStyle"> The window style of the window started by opening the URL. </param>
        /// <param name="elevated"> Specifies whether the URL should be opened with elevated privileges. </param>
        /// <returns>
        ///     true if the URL could be opened, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="url" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="url" /> is an empty string. </exception>
        /// <exception cref="UriFormatException"> <paramref name="url" /> is not a valid URI. </exception>
        public static bool OpenUrl (string url, ProcessWindowStyle windowStyle, bool elevated)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("The string is empty.", nameof(url));
            }

            Uri uri;

            try
            {
                uri = new Uri(url);
            }
            catch (Exception exception)
            {
                throw new UriFormatException(exception.Message, exception);
            }

            return WindowsShell.OpenUrl(uri, windowStyle, elevated);
        }

        /// <summary>
        ///     Opens an URL.
        /// </summary>
        /// <param name="url"> The URL to open. </param>
        /// <returns>
        ///     true if the URL could be opened, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="url" /> is null. </exception>
        public static bool OpenUrl (Uri url) => WindowsShell.OpenUrl(url, ProcessWindowStyle.Normal, false);

        /// <summary>
        ///     Opens an URL.
        /// </summary>
        /// <param name="url"> The URL to open. </param>
        /// <param name="windowStyle"> The window style of the window started by opening the URL. </param>
        /// <param name="elevated"> Specifies whether the URL should be opened with elevated privileges. </param>
        /// <returns>
        ///     true if the URL could be opened, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="url" /> is null. </exception>
        public static bool OpenUrl (Uri url, ProcessWindowStyle windowStyle, bool elevated)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            ProcessStartInfo startInfo = new ProcessStartInfo(url.ToString());

            startInfo.CreateNoWindow = false;
            startInfo.ErrorDialog = false;
            startInfo.UseShellExecute = true;
            startInfo.WindowStyle = windowStyle;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;

            if (elevated)
            {
                startInfo.Verb = WindowsShell.ElevatedVerb;
            }

            try
            {
                Process.Start(startInfo);
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
