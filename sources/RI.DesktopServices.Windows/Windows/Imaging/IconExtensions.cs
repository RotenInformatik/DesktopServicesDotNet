using System;
using System.Drawing;
using System.IO;




namespace RI.Framework.Windows.Imaging
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="Icon" /> type.
    /// </summary>
    public static class IconExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Converts an icon into a byte array.
        /// </summary>
        /// <param name="icon"> The icon. </param>
        /// <returns>
        ///     The byte array.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="icon" /> is null. </exception>
        public static byte[] ToByteArray (this Icon icon)
        {
            if (icon == null)
            {
                throw new ArgumentNullException(nameof(icon));
            }

            using (MemoryStream ms = new MemoryStream())
            {
                icon.Save(ms);

                ms.Flush();
                ms.Position = 0;

                return ms.ToArray();
            }
        }

        /// <summary>
        ///     Creates an icon from a byte array.
        /// </summary>
        /// <param name="array"> The byte array. </param>
        /// <returns>
        ///     The created icon.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="array" /> is null. </exception>
        public static Icon ToIcon (this byte[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            using (MemoryStream ms = new MemoryStream(array, false))
            {
                using (Icon icon = new Icon(ms))
                {
                    return (Icon)icon.Clone();
                }
            }
        }

        #endregion
    }
}
