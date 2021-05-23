using System;




namespace RI.Framework.IO.Printer
{
    /// <summary>
    ///     Defines the printer types which are to be retrieved using <see cref="PrinterDevice.GetPrinters" />
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    [Serializable]
    [Flags]
    public enum PrinterTypes
    {
        /// <summary>
        ///     Local connected printers (e.g. printers using a local port such as USB, network, etc.).
        /// </summary>
        Local = 0x01,

        /// <summary>
        ///     Remote printers (e.g. shared printers).
        /// </summary>
        Remote = 0x02,

        /// <summary>
        ///     All printers, local connected and remote.
        /// </summary>
        All = PrinterTypes.Local | PrinterTypes.Remote,
    }
}
