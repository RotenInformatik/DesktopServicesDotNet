using System;
using System.Drawing;
using System.Windows.Media.Imaging;




namespace RI.Framework.Windows.Imaging
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="BitmapFrame" /> type.
	/// </summary>
	public static class BitmapFrameExtensions
	{
		#region Static Methods

		/// <summary>
		///     Converts an icon to a bitmap frame.
		/// </summary>
		/// <param name="icon"> The icon. </param>
		/// <returns>
		///     The bitmap frame.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="icon" /> is null. </exception>
		public static BitmapFrame ToBitmapFrame (this Icon icon)
		{
			if (icon == null)
			{
				throw new ArgumentNullException(nameof(icon));
			}

			BitmapSource bitmapSource = icon.ToBitmapSource();

			return BitmapFrame.Create(bitmapSource);
		}

		/// <summary>
		///     Converts a bitmap to a bitmap frame.
		/// </summary>
		/// <param name="image"> The bitmap. </param>
		/// <returns>
		///     The bitmap frame.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="image" /> is null. </exception>
		public static BitmapFrame ToBitmapFrame (this Bitmap image)
		{
			if (image == null)
			{
				throw new ArgumentNullException(nameof(image));
			}

			BitmapSource bitmapSource = image.ToBitmapSource();

			return BitmapFrame.Create(bitmapSource);
		}

		/// <summary>
		///     Converts a bitmap source to a bitmap frame.
		/// </summary>
		/// <param name="image"> The bitmap source. </param>
		/// <returns>
		///     The bitmap frame.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="image" /> is null. </exception>
		public static BitmapFrame ToBitmapFrame (this BitmapSource image)
		{
			if (image == null)
			{
				throw new ArgumentNullException(nameof(image));
			}

			return BitmapFrame.Create(image.Clone());
		}

		#endregion
	}
}
