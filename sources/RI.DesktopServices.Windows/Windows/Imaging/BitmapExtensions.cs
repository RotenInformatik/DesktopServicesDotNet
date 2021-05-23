using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;




namespace RI.DesktopServices.Windows.Imaging
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="Bitmap" /> type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class BitmapExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Creates a bitmap from a byte array.
        /// </summary>
        /// <param name="array"> The byte array. </param>
        /// <returns>
        ///     The created bitmap.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="array" /> is null. </exception>
        public static Bitmap ToBitmap (this byte[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            using (MemoryStream ms = new MemoryStream(array, false))
            {
                using (Bitmap bmp = new Bitmap(ms))
                {
                    return bmp.Clone(new Rectangle(new Point(0, 0), bmp.Size), bmp.PixelFormat);
                }
            }
        }

        /// <summary>
        ///     Converts a bitmap into a BMP byte array.
        /// </summary>
        /// <param name="image"> The icon. </param>
        /// <returns>
        ///     The byte array.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="image" /> is null. </exception>
        public static byte[] ToBmpByteArray (this Bitmap image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            return image.ToByteArray(ImageFormat.Bmp);
        }

        /// <summary>
        ///     Converts a bitmap into a byte array.
        /// </summary>
        /// <param name="image"> The icon. </param>
        /// <param name="format"> The image format. </param>
        /// <returns>
        ///     The byte array.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="image" /> or <paramref name="format" /> is null. </exception>
        public static byte[] ToByteArray (this Bitmap image, ImageFormat format)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);

                ms.Flush();
                ms.Position = 0;

                return ms.ToArray();
            }
        }

        /// <summary>
        ///     Converts a bitmap to an icon.
        /// </summary>
        /// <param name="image"> The bitmap. </param>
        /// <returns>
        ///     The icon.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="image" /> is null. </exception>
        public static Icon ToIcon (this Bitmap image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            IntPtr hIcon = IntPtr.Zero;

            try
            {
                hIcon = image.GetHicon();

                using (Icon icon = Icon.FromHandle(hIcon))
                {
                    return (Icon)icon.Clone();
                }
            }
            finally
            {
                if (hIcon != IntPtr.Zero)
                {
                    BitmapExtensions.DestroyIcon(hIcon);
                }
            }
        }

        /// <summary>
        ///     Converts a bitmap into a JPEG byte array.
        /// </summary>
        /// <param name="image"> The icon. </param>
        /// <returns>
        ///     The byte array.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="image" /> is null. </exception>
        public static byte[] ToJpegByteArray (this Bitmap image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            return image.ToByteArray(ImageFormat.Jpeg);
        }

        /// <summary>
        ///     Converts a bitmap into a PNG byte array.
        /// </summary>
        /// <param name="image"> The icon. </param>
        /// <returns>
        ///     The byte array.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="image" /> is null. </exception>
        public static byte[] ToPngByteArray (this Bitmap image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            return image.ToByteArray(ImageFormat.Png);
        }

        [DllImport("user32.dll", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyIcon (IntPtr handle);

        #endregion
    }
}
