using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

using RI.Framework.Utilities;




namespace RI.Framework.Windows.Imaging
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="BitmapSource" /> type.
	/// </summary>
	public static class BitmapSourceExtensions
	{
		#region Static Methods

		/// <summary>
		///     Converts a bitmap source to a bitmap.
		/// </summary>
		/// <param name="image"> The bitmap source. </param>
		/// <returns>
		///     The bitmap.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="image" /> is null. </exception>
		public static Bitmap ToBitmap (this BitmapSource image)
		{
			if (image == null)
			{
				throw new ArgumentNullException(nameof(image));
			}

			using (MemoryStream ms = new MemoryStream())
			{
				BmpBitmapEncoder encoder = new BmpBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(image));
				encoder.Save(ms);

				ms.Flush();
				ms.Position = 0;

				using (Bitmap bmp = new Bitmap(ms))
				{
					return bmp.Clone(new Rectangle(new System.Drawing.Point(0, 0), bmp.Size), bmp.PixelFormat);
				}
			}
		}

		/// <summary>
		///     Converts an icon to a bitmap source.
		/// </summary>
		/// <param name="icon"> The icon. </param>
		/// <returns>
		///     The bitmap source.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="icon" /> is null. </exception>
		public static BitmapSource ToBitmapSource (this Icon icon)
		{
			if (icon == null)
			{
				throw new ArgumentNullException(nameof(icon));
			}

			return icon.ToBitmap().ToBitmapSource();
		}

		/// <summary>
		///     Converts a bitmap to a bitmap source.
		/// </summary>
		/// <param name="image"> The bitmap. </param>
		/// <returns>
		///     The bitmap source.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="image" /> is null. </exception>
		public static BitmapSource ToBitmapSource (this Bitmap image)
		{
			if (image == null)
			{
				throw new ArgumentNullException(nameof(image));
			}

			IntPtr hBitmap = IntPtr.Zero;
			try
			{
				hBitmap = image.GetHbitmap();

				BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

				return bitmapSource.Clone();
			}
			finally
			{
				if (hBitmap != IntPtr.Zero)
				{
					BitmapSourceExtensions.DeleteObject(hBitmap);
				}
			}
		}

		/// <summary>
		///     Creates a bitmap source from a byte array.
		/// </summary>
		/// <param name="array"> The byte array. </param>
		/// <returns>
		///     The created bitmap source.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="array" /> is null. </exception>
		public static BitmapSource ToBitmapSource (this byte[] array)
		{
			if (array == null)
			{
				throw new ArgumentNullException(nameof(array));
			}

			using (MemoryStream ms = new MemoryStream(array, false))
			{
				BitmapImage image = new BitmapImage();

				image.BeginInit();
				image.CacheOption = BitmapCacheOption.OnLoad;
				image.CreateOptions = BitmapCreateOptions.None;
				image.StreamSource = ms;
				image.EndInit();

				return image.Clone();
			}
		}

		/// <summary>
		///     Converts a bitmap source into a BMP byte array.
		/// </summary>
		/// <param name="image"> The bitmap source. </param>
		/// <returns>
		///     The byte array.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="image" /> is null. </exception>
		public static byte[] ToBmpByteArray (this BitmapSource image)
		{
			if (image == null)
			{
				throw new ArgumentNullException(nameof(image));
			}

			BmpBitmapEncoder encoder = new BmpBitmapEncoder();

			return image.ToByteArray(encoder);
		}

		/// <summary>
		///     Converts a bitmap source into a byte array.
		/// </summary>
		/// <param name="image"> The icon. </param>
		/// <param name="encoder"> The image encoder. </param>
		/// <returns>
		///     The byte array.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="image" /> or <paramref name="encoder" /> is null. </exception>
		public static byte[] ToByteArray (this BitmapSource image, BitmapEncoder encoder)
		{
			if (image == null)
			{
				throw new ArgumentNullException(nameof(image));
			}

			if (encoder == null)
			{
				throw new ArgumentNullException(nameof(encoder));
			}

			using (MemoryStream ms = new MemoryStream())
			{
				encoder.Frames.Add(BitmapFrame.Create(image));
				encoder.Save(ms);

				return ms.ToArray();
			}
		}

		/// <summary>
		///     Converts a bitmap source to an icon.
		/// </summary>
		/// <param name="image"> The bitmap source. </param>
		/// <returns>
		///     The icon.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="image" /> is null. </exception>
		public static Icon ToIcon (this BitmapSource image)
		{
			if (image == null)
			{
				throw new ArgumentNullException(nameof(image));
			}

			using (Bitmap bmp = image.ToBitmap())
			{
				return bmp.ToIcon();
			}
		}

		/// <summary>
		///     Converts a bitmap source into a JPEG byte array.
		/// </summary>
		/// <param name="image"> The bitmap source. </param>
		/// <param name="quality"> The JPEG image quality between 0.0 and 1.0. </param>
		/// <returns>
		///     The byte array.
		/// </returns>
		/// <remarks>
		///     <para>
		///         Values for <paramref name="quality" /> are clamped between 0.0 and 1.0.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="image" /> is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="quality" /> is NaN or infinity. </exception>
		public static byte[] ToJpegByteArray (this BitmapSource image, double quality)
		{
			if (image == null)
			{
				throw new ArgumentNullException(nameof(image));
			}

			if (quality.IsNanOrInfinity())
			{
				throw new ArgumentOutOfRangeException(nameof(quality));
			}

			JpegBitmapEncoder encoder = new JpegBitmapEncoder();
			encoder.QualityLevel = 1 + (int)Math.Max(0.0, Math.Min(99.0, quality * 99.0));

			return image.ToByteArray(encoder);
		}

		/// <summary>
		///     Converts a bitmap source into a PNG byte array.
		/// </summary>
		/// <param name="image"> The bitmap source. </param>
		/// <param name="interlace"> The PNG interlace options. </param>
		/// <returns>
		///     The byte array.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="image" /> is null. </exception>
		public static byte[] ToPngByteArray (this BitmapSource image, PngInterlaceOption interlace)
		{
			if (image == null)
			{
				throw new ArgumentNullException(nameof(image));
			}

			PngBitmapEncoder encoder = new PngBitmapEncoder();
			encoder.Interlace = interlace;

			return image.ToByteArray(encoder);
		}

		[DllImport("gdi32.dll", SetLastError = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool DeleteObject (IntPtr hObject);

		#endregion
	}
}
