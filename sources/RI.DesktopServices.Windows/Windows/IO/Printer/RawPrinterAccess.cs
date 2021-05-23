using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

using RI.DesktopServices.Windows.Runtime;




namespace RI.DesktopServices.Windows.IO.Printer
{
    /// <summary>
    ///     Provides raw access to a printer device.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Raw access to a printer device can be used to send unformatted raw data to the printer.
    ///         The data is therefore printer-specific.
    ///     </para>
    ///     <para>
    ///         <see cref="RawPrinterAccess" /> is based on an instance of <see cref="PrinterDevice" /> which must be obtained
    ///         first.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class RawPrinterAccess : IDisposable
    {
        #region Constants

        private const string RawDataType = "RAW";

        #endregion




        #region Static Methods

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true,
                   CallingConvention = CallingConvention.StdCall)]
        private static extern bool ClosePrinter (IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true,
                   CallingConvention = CallingConvention.StdCall)]
        private static extern bool EndDocPrinter (IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true,
                   CallingConvention = CallingConvention.StdCall)]
        private static extern bool EndPagePrinter (IntPtr hPrinter);

        [DllImport("winspool.drv", EntryPoint = "EnumPrinters", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool EnumPrinters (uint flags, string name, uint level, IntPtr pPrinterEnum, uint cbBuf,
                                                 ref uint pcbNeeded, ref uint pcReturned);

        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi,
                   ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool OpenPrinter ([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter,
                                                IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi,
                   ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool StartDocPrinter (IntPtr hPrinter, int level, [In] [MarshalAs(UnmanagedType.LPStruct)]
                                                    DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true,
                   CallingConvention = CallingConvention.StdCall)]
        private static extern bool StartPagePrinter (IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true,
                   CallingConvention = CallingConvention.StdCall)]
        private static extern bool WritePrinter (IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="RawPrinterAccess" />.
        /// </summary>
        /// <param name="printerDevice"> The printer device. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="printerDevice" /> is null. </exception>
        public RawPrinterAccess (PrinterDevice printerDevice)
        {
            if (printerDevice == null)
            {
                throw new ArgumentNullException(nameof(printerDevice));
            }

            this.PrinterDevice = printerDevice;

            this.PrinterHandle = IntPtr.Zero;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets whether the printer has been opened for raw access.
        /// </summary>
        /// <value>
        ///     true if the printer is open for raw access, false otherwise.
        /// </value>
        public bool IsOpen => this.PrinterHandle != IntPtr.Zero;

        /// <summary>
        ///     Gets the used printer device.
        /// </summary>
        /// <value>
        ///     The used printer device.
        /// </value>
        public PrinterDevice PrinterDevice { get; }

        private IntPtr PrinterHandle { get; set; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Closes the printers raw access.
        /// </summary>
        public void Close ()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Opens the raw access to the printer and starts printing a new document.
        /// </summary>
        /// <param name="documentName"> The name of the document to print. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="documentName" /> is only used for informational purposes, e.g. it is displayed in the Windows
        ///         printer window which lists the document being printed.
        ///     </para>
        ///     <para>
        ///         Printing does not start until <see cref="WriteString(string)" />, <see cref="WriteString(string,Encoding)" />,
        ///         or <see cref="WriteBytes" /> is called.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="documentName" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="documentName" /> is an empty string. </exception>
        /// <exception cref="InvalidOperationException"> The printer is already open. </exception>
        /// <exception cref="Win32Exception"> A system error ocurred while opening the printer or starting a new document. </exception>
        public void Open (string documentName)
        {
            if (documentName == null)
            {
                throw new ArgumentNullException(nameof(documentName));
            }

            if (string.IsNullOrWhiteSpace(documentName))
            {
                throw new ArgumentException("The string is empty.", nameof(documentName));
            }

            if (this.IsOpen)
            {
                throw new InvalidOperationException("The printer is already open.");
            }

            GC.ReRegisterForFinalize(this);

            bool success = false;

            try
            {
                IntPtr hPrinter;

                if (RawPrinterAccess.OpenPrinter(this.PrinterDevice.PrinterName.Normalize(), out hPrinter, IntPtr.Zero))
                {
                    this.PrinterHandle = hPrinter;

                    DOCINFOA di = new DOCINFOA();
                    di.pDocName = documentName;
                    di.pDataType = RawPrinterAccess.RawDataType;

                    if (RawPrinterAccess.StartDocPrinter(hPrinter, 1, di))
                    {
                        if (RawPrinterAccess.StartPagePrinter(hPrinter))
                        {
                            success = true;
                            return;
                        }
                    }
                }

                int errorCode = WindowsApi.GetLastErrorCode();
                string errorMessage = WindowsApi.GetErrorMessage(errorCode);
                throw new Win32Exception(errorCode, errorMessage);
            }
            finally
            {
                if (!success)
                {
                    this.Close();
                }
            }
        }

        /// <summary>
        ///     Writes raw data as bytes to the printer.
        /// </summary>
        /// <param name="data"> The raw data to write. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="data" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The printer is not open. </exception>
        /// <exception cref="Win32Exception"> A system error ocurred while writing to the printer. </exception>
        public void WriteBytes (byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!this.IsOpen)
            {
                throw new InvalidOperationException("The printer is not open.");
            }

            IntPtr pointer = IntPtr.Zero;

            try
            {
                pointer = Marshal.AllocCoTaskMem(data.Length);
                Marshal.Copy(data, 0, pointer, data.Length);
                this.SendBytes(pointer, data.Length);
            }
            finally
            {
                if (pointer != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pointer);
                }
            }
        }

        /// <summary>
        ///     Writes raw data as a string to the printer.
        /// </summary>
        /// <param name="data"> The raw data to write. </param>
        /// <remarks>
        ///     <para>
        ///         <see cref="Encoding" />.<see cref="Encoding.Default" /> is used.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="data" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The printer is not open. </exception>
        /// <exception cref="Win32Exception"> A system error ocurred while writing to the printer. </exception>
        public void WriteString (string data)
        {
            this.WriteString(data, null);
        }

        /// <summary>
        ///     Writes raw data as bytes to the printer.
        /// </summary>
        /// <param name="data"> The raw data to write. </param>
        /// <param name="encoding"> The encoding to use. </param>
        /// <remarks>
        ///     <para>
        ///         <see cref="Encoding" />.<see cref="Encoding.Default" /> is used if <paramref name="encoding" /> is null.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="data" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The printer is not open. </exception>
        /// <exception cref="Win32Exception"> A system error ocurred while writing to the printer. </exception>
        public void WriteString (string data, Encoding encoding)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!this.IsOpen)
            {
                throw new InvalidOperationException("The printer is not open.");
            }

            byte[] encodedBytes = (encoding ?? Encoding.Default).GetBytes(data);

            byte[] finalBytes = new byte[encodedBytes.Length + 1];
            Array.Copy(encodedBytes, 0, finalBytes, 0, encodedBytes.Length);
            finalBytes[encodedBytes.Length] = (byte)'\0';

            this.WriteBytes(finalBytes);
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private void Dispose (bool disposing)
        {
            if (this.PrinterHandle != IntPtr.Zero)
            {
                RawPrinterAccess.EndPagePrinter(this.PrinterHandle);
                RawPrinterAccess.EndDocPrinter(this.PrinterHandle);
                RawPrinterAccess.ClosePrinter(this.PrinterHandle);
                this.PrinterHandle = IntPtr.Zero;
            }
        }

        private void SendBytes (IntPtr pointer, int byteCount)
        {
            if (!RawPrinterAccess.WritePrinter(this.PrinterHandle, pointer, byteCount, out _))
            {
                int errorCode = WindowsApi.GetLastErrorCode();
                string errorMessage = WindowsApi.GetErrorMessage(errorCode);
                throw new Win32Exception(errorCode, errorMessage);
            }
        }

        #endregion




        #region Overrides

        /// <summary>
        ///     Garbage collects this instance of <see cref="RawPrinterAccess" />.
        /// </summary>
        ~RawPrinterAccess ()
        {
            this.Dispose(false);
        }

        #endregion




        #region Interface: IDisposable

        /// <inheritdoc />
        void IDisposable.Dispose ()
        {
            this.Close();
        }

        #endregion




        #region Type: DOCINFOA

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "NotAccessedField.Local")]
        private class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;

            [MarshalAs(UnmanagedType.LPStr)]
            #pragma warning disable 649
            public string pOutputFile;
            #pragma warning restore 649

            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }

        #endregion
    }
}
