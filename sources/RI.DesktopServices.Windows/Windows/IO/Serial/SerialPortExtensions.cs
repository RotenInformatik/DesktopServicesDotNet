using System;




namespace RI.Framework.IO.Serial
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="SerialPort" /> type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class SerialPortExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Gets the serial port instance for the serial port.
        /// </summary>
        /// <param name="serialPort"> The serial port. </param>
        /// <returns>
        ///     The serial port instance.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="serialPort" /> is null. </exception>
        public static SerialPortInstance GetSerialPortInstance (this SerialPort serialPort)
        {
            if (serialPort == null)
            {
                throw new ArgumentNullException(nameof(serialPort));
            }

            return new SerialPortInstance(serialPort.PortName);
        }

        #endregion
    }
}
