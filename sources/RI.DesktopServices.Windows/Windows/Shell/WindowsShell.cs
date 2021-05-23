using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;




namespace RI.Framework.Windows.Shell
{
    /// <summary>
    ///     Provides utilities for working with the Windows shell environment.
    /// </summary>
    public static class WindowsShell
    {
        #region Static Fields

        private static readonly string BatchFileExtension = ".bat";

        private static readonly string CommandPromptArguments = "/k \"cd /d {0}\"";

        private static readonly FilePath CommandPromptExecutable = new FilePath("cmd.exe");

        private static readonly string ElevatedVerb = new FilePath("runas");

        private static readonly FilePath ExplorerExecutable = new FilePath("explorer.exe");

        private static readonly FilePath SystemInfoExecutable = new FilePath("msinfo32.exe");

        private static readonly FilePath TaskManagerExecutable = new FilePath("taskmgr.exe");

        #endregion




        #region Static Methods

        /// <summary>
        ///     Executes batch commands.
        /// </summary>
        /// <param name="commands"> The batch commands. </param>
        /// <param name="workingDirectory"> The used working directory. Can be null to use the current directory. </param>
        /// <returns>
        ///     The <see cref="Process" /> if the commands could be started successfully, null otherwise.
        /// </returns>
        /// <remarks>
        ///     <note type="important">
        ///         The standard output is redirected.
        ///         Therefore, you must read the <see cref="Process.StandardOutput" /> reader.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="commands" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="commands" /> is an empty string. </exception>
        public static Process ExecuteBatchCommands (string commands, DirectoryPath workingDirectory)
        {
            if (commands == null)
            {
                throw new ArgumentNullException(nameof(commands));
            }

            if (commands.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(commands));
            }

            workingDirectory = workingDirectory ?? DirectoryPath.GetCurrentDirectory();
            string directory = Environment.ExpandEnvironmentVariables(workingDirectory);

            TemporaryFile tempFile = new TemporaryFile(WindowsShell.BatchFileExtension);
            tempFile.File.WriteText(commands, Encoding.Default);

            ProcessStartInfo startInfo =
                new ProcessStartInfo(WindowsShell.CommandPromptExecutable, "/c call \"" + tempFile.File + "\"");

            startInfo.CreateNoWindow = true;
            startInfo.ErrorDialog = false;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = directory;
            startInfo.RedirectStandardOutput = true;
            startInfo.StandardOutputEncoding = Encoding.UTF8;

            Process process;

            try
            {
                process = Process.Start(startInfo);
            }
            catch
            {
                return null;
            }

            if (process == null)
            {
                return null;
            }

            process.EnableRaisingEvents = true;

            EventHandler exitHandler = null;

            exitHandler = (sender, args) =>
            {
                process.Exited -= exitHandler;
                tempFile.Delete();
            };

            process.Exited += exitHandler;

            if (process.HasExited)
            {
                process.Exited -= exitHandler;
                tempFile.Delete();
            }

            return process;
        }

        /// <summary>
        ///     Executes a batch script file.
        /// </summary>
        /// <param name="scriptFile"> The batch script file. </param>
        /// <param name="scriptArguments"> The batch script arguments or null if no arguments are used. </param>
        /// <param name="workingDirectory"> The used working directory. Can be null to use the current directory. </param>
        /// <returns>
        ///     The <see cref="Process" /> if the script file could be started successfully, null otherwise.
        /// </returns>
        /// <remarks>
        ///     <note type="important">
        ///         The standard output is redirected.
        ///         Therefore, you must read the <see cref="Process.StandardOutput" /> reader.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="scriptFile" /> is null. </exception>
        /// <exception cref="InvalidPathArgumentException"> <paramref name="scriptFile" /> is not a valid path. </exception>
        /// <exception cref="FileNotFoundException"> <paramref name="scriptFile" /> does not exist. </exception>
        public static Process ExecuteBatchScript (FilePath scriptFile, string scriptArguments,
                                                  DirectoryPath workingDirectory)
        {
            if (scriptFile == null)
            {
                throw new ArgumentNullException(nameof(scriptFile));
            }

            if (!scriptFile.IsRealFile)
            {
                throw new InvalidPathArgumentException(nameof(scriptFile));
            }

            if (!scriptFile.Exists)
            {
                throw new FileNotFoundException("Script file not found.", scriptFile);
            }

            workingDirectory = workingDirectory ?? DirectoryPath.GetCurrentDirectory();
            string directory = Environment.ExpandEnvironmentVariables(workingDirectory);

            StringBuilder arguments = new StringBuilder();
            arguments.Append("/c call \"" + scriptFile + "\"");

            if (!scriptArguments.IsNullOrEmptyOrWhitespace())
            {
                arguments.Append(" ");
                arguments.Append(Environment.ExpandEnvironmentVariables(scriptArguments));
            }

            ProcessStartInfo startInfo =
                new ProcessStartInfo(WindowsShell.CommandPromptExecutable, arguments.ToString());

            startInfo.CreateNoWindow = true;
            startInfo.ErrorDialog = false;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = directory;
            startInfo.RedirectStandardOutput = true;
            startInfo.StandardOutputEncoding = Encoding.UTF8;

            try
            {
                return Process.Start(startInfo);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Executes a console command.
        /// </summary>
        /// <param name="command"> The command to execute. </param>
        /// <param name="arguments"> The arguments. Can be null or an empty string if not used. </param>
        /// <param name="workingDirectory"> The used working directory. Can be null to use the current directory. </param>
        /// <returns>
        ///     The <see cref="Process" /> if the command could be started successfully, null otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         Environment variables will be resolved for <paramref name="command" />, <paramref name="arguments" />, and
        ///         <paramref name="workingDirectory" />.
        ///     </para>
        ///     <note type="important">
        ///         The standard output is redirected.
        ///         Therefore, you must read the <see cref="Process.StandardOutput" /> reader.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="command" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="command" /> is an empty string. </exception>
        public static Process ExecuteConsoleCommand (string command, string arguments, DirectoryPath workingDirectory)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(command));
            }

            arguments = arguments ?? string.Empty;
            workingDirectory = workingDirectory ?? DirectoryPath.GetCurrentDirectory();

            command = Environment.ExpandEnvironmentVariables(command);
            arguments = Environment.ExpandEnvironmentVariables(arguments);

            string directory = Environment.ExpandEnvironmentVariables(workingDirectory);

            ProcessStartInfo startInfo = new ProcessStartInfo(command, arguments);
            startInfo.CreateNoWindow = true;
            startInfo.ErrorDialog = false;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = directory;
            startInfo.RedirectStandardOutput = true;
            startInfo.StandardOutputEncoding = Encoding.UTF8;

            try
            {
                return Process.Start(startInfo);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Executes a shell command.
        /// </summary>
        /// <param name="command"> The command, program, file, or folder to execute. </param>
        /// <param name="arguments"> The arguments. Can be null or an empty string if not used. </param>
        /// <param name="verb"> The verb. Can be null or an empty string if not used. </param>
        /// <param name="workingDirectory"> The used working directory. Can be null to use the current directory. </param>
        /// <param name="windowStyle"> The window style of the opened program window (if any). </param>
        /// <returns>
        ///     The <see cref="Process" /> if the command could be started successfully, null otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         Environment variables will be resolved for <paramref name="command" />, <paramref name="arguments" />,
        ///         <paramref name="verb" />, and <paramref name="workingDirectory" />.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="command" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="command" /> is an empty string. </exception>
        public static Process ExecuteShellCommand (string command, string arguments, string verb,
                                                   DirectoryPath workingDirectory, ProcessWindowStyle windowStyle)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(command));
            }

            arguments = arguments ?? string.Empty;
            verb = verb ?? string.Empty;
            workingDirectory = workingDirectory ?? DirectoryPath.GetCurrentDirectory();

            command = Environment.ExpandEnvironmentVariables(command);
            arguments = Environment.ExpandEnvironmentVariables(arguments);
            verb = Environment.ExpandEnvironmentVariables(verb);

            string directory = Environment.ExpandEnvironmentVariables(workingDirectory);

            ProcessStartInfo startInfo = new ProcessStartInfo(command, arguments);
            startInfo.CreateNoWindow = false;
            startInfo.ErrorDialog = false;
            startInfo.UseShellExecute = true;
            startInfo.WindowStyle = windowStyle;
            startInfo.WorkingDirectory = directory;
            startInfo.Verb = verb;

            try
            {
                return Process.Start(startInfo);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Opens a command prompt.
        /// </summary>
        /// <returns>
        ///     true if the command prompt could be opened, false otherwise.
        /// </returns>
        public static bool OpenCommandPrompt ()
        {
            return WindowsShell.OpenCommandPrompt(null, false);
        }

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
        public static bool OpenCommandPrompt (DirectoryPath workingDirectory, bool elevated)
        {
            workingDirectory = workingDirectory ?? DirectoryPath.GetCurrentDirectory();

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
        public static bool OpenExplorer ()
        {
            return WindowsShell.OpenExplorer(ProcessWindowStyle.Normal, false);
        }

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
        /// <exception cref="InvalidPathArgumentException"> <paramref name="filePath" /> has wildcards. </exception>
        public static bool OpenFile (FilePath filePath)
        {
            return WindowsShell.OpenFile(filePath, ProcessWindowStyle.Normal, false);
        }

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
        /// <exception cref="InvalidPathArgumentException"> <paramref name="filePath" /> has wildcards. </exception>
        public static bool OpenFile (FilePath filePath, ProcessWindowStyle windowStyle, bool elevated)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (filePath.HasWildcards)
            {
                throw new InvalidPathArgumentException(nameof(filePath));
            }

            string file = Environment.ExpandEnvironmentVariables(filePath);
            string directory = Environment.ExpandEnvironmentVariables(filePath.Directory);

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
        /// <exception cref="InvalidPathArgumentException"> <paramref name="folderPath" /> has wildcards. </exception>
        public static bool OpenFolder (DirectoryPath folderPath)
        {
            return WindowsShell.OpenFolder(folderPath, ProcessWindowStyle.Normal, false);
        }

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
        /// <exception cref="InvalidPathArgumentException"> <paramref name="folderPath" /> has wildcards. </exception>
        public static bool OpenFolder (DirectoryPath folderPath, ProcessWindowStyle windowStyle, bool elevated)
        {
            if (folderPath == null)
            {
                throw new ArgumentNullException(nameof(folderPath));
            }

            if (folderPath.HasWildcards)
            {
                throw new InvalidPathArgumentException(nameof(folderPath));
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
        public static bool OpenSystemInfo ()
        {
            return WindowsShell.OpenSystemInfo(ProcessWindowStyle.Normal, false);
        }

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
        public static bool OpenTaskManager ()
        {
            return WindowsShell.OpenTaskManager(ProcessWindowStyle.Normal, false);
        }

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
        /// <exception cref="EmptyStringArgumentException"> <paramref name="url" /> is an empty string. </exception>
        /// <exception cref="UriFormatException"> <paramref name="url" /> is not a valid URI. </exception>
        public static bool OpenUrl (string url)
        {
            return WindowsShell.OpenUrl(url, ProcessWindowStyle.Normal, false);
        }

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
        /// <exception cref="EmptyStringArgumentException"> <paramref name="url" /> is an empty string. </exception>
        /// <exception cref="UriFormatException"> <paramref name="url" /> is not a valid URI. </exception>
        public static bool OpenUrl (string url, ProcessWindowStyle windowStyle, bool elevated)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (url.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(url));
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
        public static bool OpenUrl (Uri url)
        {
            return WindowsShell.OpenUrl(url, ProcessWindowStyle.Normal, false);
        }

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
