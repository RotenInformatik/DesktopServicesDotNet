using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;




namespace RI.DesktopServices.Windows.Runtime
{
    /// <summary>
    ///     Provides utilities for working with the native Windows API (WIN32).
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class WindowsApi
    {
        #region Constants

        private const uint FormatMessageAllocateBuffer = 0x00000100;

        private const uint FormatMessageFromSystem = 0x00001000;

        private const uint FormatMessageIgnoreInserts = 0x00000200;

        #endregion




        #region Static Methods

        /// <summary>
        ///     Gets the error message for a specified Windows API error code.
        /// </summary>
        /// <param name="errorCode"> The Windows API error code. </param>
        /// <returns>
        ///     The error message for the specified error code.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The current UI culture is used to determine which language to use for the error message.
        ///     </para>
        /// </remarks>
        /// <exception cref="Win32Exception"> The error message could not be retrieved. </exception>
        public static string GetErrorMessage (int errorCode) => WindowsApi.GetErrorMessage(errorCode, null);

        /// <summary>
        /// </summary>
        /// <param name="errorCode"> The Windows API error code. </param>
        /// <param name="language"> The culture which defines the language to use for the error message. </param>
        /// <returns>
        ///     The error message for the specified error code.
        /// </returns>
        /// <exception cref="Win32Exception"> The error message could not be retrieved. </exception>
        public static string GetErrorMessage (int errorCode, CultureInfo language)
        {
            IntPtr messageBuffer = IntPtr.Zero;

            try
            {
                uint chars =
                    WindowsApi
                        .FormatMessage(WindowsApi.FormatMessageFromSystem | WindowsApi.FormatMessageAllocateBuffer | WindowsApi.FormatMessageIgnoreInserts,
                                       IntPtr.Zero, (uint)errorCode, language == null ? 0 : language.LCID,
                                       ref messageBuffer, 0, IntPtr.Zero);

                if (chars == 0)
                {
                    throw new Win32Exception(WindowsApi.GetLastErrorCode());
                }

                string message = Marshal.PtrToStringUni(messageBuffer);

                return message;
            }
            finally
            {
                if (messageBuffer != IntPtr.Zero)
                {
                    WindowsApi.LocalFree(messageBuffer);
                }
            }
        }

        /// <summary>
        ///     Gets the Windows API error code which was set during the last call to the Windows API.
        /// </summary>
        /// <returns>
        ///     The error code set by the last call to the Windows API or zero if no error code was set (or no error occurred
        ///     respectively).
        /// </returns>
        public static int GetLastErrorCode ()
        {
            return Marshal.GetLastWin32Error();
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern uint FormatMessage (uint dwFlags, IntPtr lpSource, uint dwMessageId, int dwLanguageId,
                                                  ref IntPtr lpBuffer, uint nSize, IntPtr pArguments);

        [DllImport("kernel32.dll", SetLastError = false)]
        private static extern IntPtr LocalFree (IntPtr hMem);

        #endregion
    }
}
