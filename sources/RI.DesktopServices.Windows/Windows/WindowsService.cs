using System;
using System.Diagnostics;

using RI.Framework.Windows.Shell;




namespace RI.Framework.Windows
{
    /// <summary>
    ///     Provides utilities for working with Windows services.
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         Note that service control actions as provided in <see cref="WindowsService" /> requires certain permissions.
    ///     </note>
    ///     <note type="important">
    ///         Not using the service controller delegates the service actions into a batch file, executed in its own process.
    ///         Therefore, service control actions not using the service controller will silently fail on errors.
    ///         However, not using the service controller can be used inside a service to control itself.
    ///     </note>
    /// </remarks>
    public static class WindowsService
    {
        #region Static Methods

        /// <summary>
        /// </summary>
        /// <param name="serviceName"> The name of the service to restart. </param>
        /// <param name="useServiceController">
        ///     Specifies whether the service controller (true) or a batch command (false) should
        ///     be used.
        /// </param>
        /// <param name="timeout">
        ///     The timeout for waiting for the service to restart or null to wait indefinitely. Ignored if
        ///     <paramref name="useServiceController" /> is false.
        /// </param>
        /// <remarks>
        ///     <note type="note">
        ///         See the remarks of <see cref="WindowsService" /> for more important information about service control actions
        ///         provided by <see cref="WindowsService" />.
        ///     </note>
        ///     <para>
        ///         <paramref name="timeout" /> must have a value if <paramref name="useServiceController" /> is true.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="serviceName" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="serviceName" /> is an empty string. </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="useServiceController" /> is true and <paramref name="timeout" />
        ///     is null.
        /// </exception>
        /// <exception cref="System.ServiceProcess.TimeoutException">
        ///     <paramref name="useServiceController" /> is true,
        ///     <paramref name="timeout" /> provided a value, and the timeout expired while waiting.
        /// </exception>
        public static void RestartService (string serviceName, bool useServiceController, TimeSpan? timeout)
        {
            if (serviceName == null)
            {
                throw new ArgumentNullException(nameof(serviceName));
            }

            if (serviceName.IsNullOrEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(serviceName));
            }

            if (useServiceController && (!timeout.HasValue))
            {
                throw new
                    ArgumentException("A timeout must be specified when service controller is used to restart a service.",
                                      nameof(timeout));
            }

            if (useServiceController)
            {
                ServiceController controller = new ServiceController(serviceName);
                controller.Stop();
                controller.WaitForStatus(ServiceControllerStatus.Stopped, timeout.Value);
                controller.Start();
                controller.WaitForStatus(ServiceControllerStatus.Stopped, timeout.Value);
            }
            else
            {
                WindowsService.ExecuteCommands("net stop \"" + serviceName + "\"", "net start \"" + serviceName + "\"");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="serviceName"> The name of the service to start. </param>
        /// <param name="useServiceController">
        ///     Specifies whether the service controller (true) or a batch command (false) should
        ///     be used.
        /// </param>
        /// <param name="timeout">
        ///     The timeout for waiting for the service to start or null to wait indefinitely. Ignored if
        ///     <paramref name="useServiceController" /> is false.
        /// </param>
        /// <remarks>
        ///     <note type="note">
        ///         See the remarks of <see cref="WindowsService" /> for more important information about service control actions
        ///         provided by <see cref="WindowsService" />.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="serviceName" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="serviceName" /> is an empty string. </exception>
        /// <exception cref="System.ServiceProcess.TimeoutException">
        ///     <paramref name="useServiceController" /> is true,
        ///     <paramref name="timeout" /> provided a value, and the timeout expired while waiting.
        /// </exception>
        public static void StartService (string serviceName, bool useServiceController, TimeSpan? timeout)
        {
            if (serviceName == null)
            {
                throw new ArgumentNullException(nameof(serviceName));
            }

            if (serviceName.IsNullOrEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(serviceName));
            }

            if (useServiceController)
            {
                ServiceController controller = new ServiceController(serviceName);
                controller.Start();

                if (timeout.HasValue)
                {
                    controller.WaitForStatus(ServiceControllerStatus.Running, timeout.Value);
                }
            }
            else
            {
                WindowsService.ExecuteCommands("net start \"" + serviceName + "\"");
            }
        }

        /// <summary>
        ///     Stops a service.
        /// </summary>
        /// <param name="serviceName"> The name of the service to stop. </param>
        /// <param name="useServiceController">
        ///     Specifies whether the service controller (true) or a batch command (false) should
        ///     be used.
        /// </param>
        /// <param name="timeout">
        ///     The timeout for waiting for the service to stop or null to wait indefinitely. Ignored if
        ///     <paramref name="useServiceController" /> is false.
        /// </param>
        /// <remarks>
        ///     <note type="note">
        ///         See the remarks of <see cref="WindowsService" /> for more important information about service control actions
        ///         provided by <see cref="WindowsService" />.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="serviceName" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="serviceName" /> is an empty string. </exception>
        /// <exception cref="System.ServiceProcess.TimeoutException">
        ///     <paramref name="useServiceController" /> is true,
        ///     <paramref name="timeout" /> provided a value, and the timeout expired while waiting.
        /// </exception>
        public static void StopService (string serviceName, bool useServiceController, TimeSpan? timeout)
        {
            if (serviceName == null)
            {
                throw new ArgumentNullException(nameof(serviceName));
            }

            if (serviceName.IsNullOrEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(serviceName));
            }

            if (useServiceController)
            {
                ServiceController controller = new ServiceController(serviceName);
                controller.Stop();

                if (timeout.HasValue)
                {
                    controller.WaitForStatus(ServiceControllerStatus.Stopped, timeout.Value);
                }
            }
            else
            {
                WindowsService.ExecuteCommands("net stop \"" + serviceName + "\"");
            }
        }

        private static void ExecuteCommands (params string[] commands)
        {
            string fileName = "cmd.exe";
            string arguments = "/C " + commands.Join(" && ");
            Process process = WindowsShell.ExecuteConsoleCommand(fileName, arguments, null);
            process?.Dispose();
        }

        #endregion
    }
}
