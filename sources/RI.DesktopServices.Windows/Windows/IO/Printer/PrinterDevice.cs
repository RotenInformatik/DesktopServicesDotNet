using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

using RI.DesktopServices.Windows.Interop;
using RI.DesktopServices.Windows.Runtime;




namespace RI.DesktopServices.Windows.IO.Printer
{
    /// <summary>
    ///     Provides information about printer devices.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="PrinterDevice" /> provides various information about printer devices available to the local machine.
    ///         The available printer devices can be retrieved using <see cref="GetPrinters" />.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class PrinterDevice
    {
        #region Static Methods

        /// <summary>
        ///     Gets all printer devices currently available to the local machine.
        /// </summary>
        /// <param name="printerTypes"> The types of printers to get. </param>
        /// <returns>
        ///     The array of available printer device information.
        ///     An empty array is returned if no printers are currently available.
        /// </returns>
        /// <exception cref="Win32Exception"> A system error ocurred during enumeration of the printer devices. </exception>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static PrinterDevice[] GetPrinters (PrinterTypes printerTypes)
        {
            PrinterEnumFlags flags = 0;

            if ((printerTypes & PrinterTypes.Local) == PrinterTypes.Local)
            {
                flags |= PrinterEnumFlags.PRINTER_ENUM_LOCAL;
            }

            if ((printerTypes & PrinterTypes.Remote) == PrinterTypes.Remote)
            {
                flags |= PrinterEnumFlags.PRINTER_ENUM_REMOTE;
            }

            uint cbNeeded = 0;
            uint cReturned = 0;

            if (PrinterDevice.EnumPrinters((uint)flags, null, 2, IntPtr.Zero, 0, ref cbNeeded, ref cReturned))
            {
                if (cbNeeded == 0)
                {
                    return new PrinterDevice[0];
                }
            }

            int errorCode = WindowsApi.GetLastErrorCode();

            if (errorCode == (int)WindowsError.ErrorInsufficientBuffer)
            {
                IntPtr pAddr = IntPtr.Zero;

                try
                {
                    pAddr = Marshal.AllocHGlobal((int)cbNeeded);

                    if (PrinterDevice.EnumPrinters((uint)flags, null, 2, pAddr, cbNeeded, ref cbNeeded, ref cReturned))
                    {
                        PRINTER_INFO_2[] printerInfo = new PRINTER_INFO_2[cReturned];

                        long address = pAddr.ToInt64();
                        long addressIncrement = Marshal.SizeOf(typeof(PRINTER_INFO_2));

                        for (int i1 = 0; i1 < cReturned; i1++)
                        {
                            printerInfo[i1] =
                                (PRINTER_INFO_2)Marshal.PtrToStructure(new IntPtr(address), typeof(PRINTER_INFO_2));

                            address += addressIncrement;
                        }

                        return (from x in printerInfo
                                select new PrinterDevice(x)).ToArray();
                    }

                    errorCode = WindowsApi.GetLastErrorCode();
                }
                finally
                {
                    if (pAddr != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(pAddr);
                    }
                }
            }

            string errorMessage = WindowsApi.GetErrorMessage(errorCode);
            throw new Win32Exception(errorCode, errorMessage);
        }

        [DllImport("winspool.drv", EntryPoint = "EnumPrinters", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool EnumPrinters (uint flags, [MarshalAs(UnmanagedType.LPTStr)] string name, uint level,
                                                 IntPtr pPrinterEnum, uint cbBuf, ref uint pcbNeeded,
                                                 ref uint pcReturned);

        #endregion




        #region Instance Constructor/Destructor

        private PrinterDevice (PRINTER_INFO_2 nativePrinterInfo)
        {
            this.NativePrinterInfo = nativePrinterInfo;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the comment of the printer.
        /// </summary>
        /// <value>
        ///     The comment of the printer (a brief description).
        /// </value>
        public string Comment => this.NativePrinterInfo.pComment;

        /// <summary>
        ///     Gets the name of the printer driver.
        /// </summary>
        /// <value>
        ///     The name of the printer driver.
        /// </value>
        public string DriverName => this.NativePrinterInfo.pDriverName;

        /// <summary>
        ///     Gets the physical location of the printer.
        /// </summary>
        /// <value>
        ///     The physical location of the printer.
        /// </value>
        public string Location => this.NativePrinterInfo.pLocation;

        /// <summary>
        ///     Gets the name of the port used by the printer.
        /// </summary>
        /// <value>
        ///     The name of the port used by the printer.
        /// </value>
        public string PortName => this.NativePrinterInfo.pPortName;

        /// <summary>
        ///     Gets the name of the printer.
        /// </summary>
        /// <value>
        ///     The name of the printer.
        /// </value>
        public string PrinterName => this.NativePrinterInfo.pPrinterName;

        /// <summary>
        ///     Gets the name of the print processor.
        /// </summary>
        /// <value>
        ///     The name of the print processor.
        /// </value>
        public string ProcessorName => this.NativePrinterInfo.pPrintProcessor;

        /// <summary>
        ///     Gets the name of the server which controls the printer.
        /// </summary>
        /// <value>
        ///     The name of the server which controls the printer or null if it is a local printer.
        /// </value>
        public string ServerName => this.NativePrinterInfo.pServerName;

        /// <summary>
        ///     Gets the name of the printer share.
        /// </summary>
        /// <value>
        ///     The name of the printer share or null if the printer is not a shared printer.
        /// </value>
        public string ShareName => this.NativePrinterInfo.pShareName;

        private PRINTER_INFO_2 NativePrinterInfo { get; }

        #endregion




        #region Type: PRINTER_INFO_2

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private struct PRINTER_INFO_2
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pServerName;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pPrinterName;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pShareName;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pPortName;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDriverName;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pComment;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pLocation;

            public IntPtr pDevMode;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pSepFile;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pPrintProcessor;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDatatype;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pParameters;

            public IntPtr pSecurityDescriptor;

            public uint Attributes;

            public uint Priority;

            public uint DefaultPriority;

            public uint StartTime;

            public uint UntilTime;

            public uint Status;

            public uint cJobs;

            public uint AveragePPM;
        }

        #endregion




        #region Type: PrinterEnumFlags

        [Serializable]
        [Flags]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private enum PrinterEnumFlags
        {
            PRINTER_ENUM_DEFAULT = 0x00000001,

            PRINTER_ENUM_LOCAL = 0x00000002,

            PRINTER_ENUM_CONNECTIONS = 0x00000004,

            PRINTER_ENUM_FAVORITE = 0x00000004,

            PRINTER_ENUM_NAME = 0x00000008,

            PRINTER_ENUM_REMOTE = 0x00000010,

            PRINTER_ENUM_SHARED = 0x00000020,

            PRINTER_ENUM_NETWORK = 0x00000040,

            PRINTER_ENUM_EXPAND = 0x00004000,

            PRINTER_ENUM_CONTAINER = 0x00008000,

            PRINTER_ENUM_ICONMASK = 0x00ff0000,

            PRINTER_ENUM_ICON1 = 0x00010000,

            PRINTER_ENUM_ICON2 = 0x00020000,

            PRINTER_ENUM_ICON3 = 0x00040000,

            PRINTER_ENUM_ICON4 = 0x00080000,

            PRINTER_ENUM_ICON5 = 0x00100000,

            PRINTER_ENUM_ICON6 = 0x00200000,

            PRINTER_ENUM_ICON7 = 0x00400000,

            PRINTER_ENUM_ICON8 = 0x00800000,

            PRINTER_ENUM_HIDE = 0x01000000,

            PRINTER_ENUM_CATEGORY_ALL = 0x02000000,

            PRINTER_ENUM_CATEGORY_3D = 0x04000000,
        }

        #endregion
    }
}
