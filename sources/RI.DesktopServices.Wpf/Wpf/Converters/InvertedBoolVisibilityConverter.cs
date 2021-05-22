using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Windows.Wpf.Converters
{
	/// <summary>
	///     Implements a converter between <see cref="bool" /> and <see cref="Visibility" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="InvertedBoolVisibilityConverter" /> is used when a boolen value needs to be used as the visibility for a WPF element.
	///         If the value is false, the converter returns <see cref="Visibility.Visible" />.
	///         If the value is true, the converter returns its parameter of type <see cref="Visibility" />.
	///         This allows to choose whether <see cref="Visibility.Hidden" /> or <see cref="Visibility.Collapsed" /> is used when the boolean value is true.
	///     </para>
	///     <para>
	///         The converter only supports conversion from <see cref="bool" /> (the source) to <see cref="Visibility" /> (the target).
	///     </para>
	/// </remarks>
	[ValueConversion(typeof(bool), typeof(Visibility), ParameterType = typeof(Visibility))]
	public sealed class InvertedBoolVisibilityConverter : IValueConverter
	{
		#region Interface: IValueConverter

		/// <inheritdoc />
		public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is bool))
			{
				throw new InvalidTypeArgumentException(nameof(value));
			}

			if (!(parameter is Visibility))
			{
				throw new InvalidTypeArgumentException(nameof(parameter));
			}

			return ((bool)value) ? (Visibility)parameter : Visibility.Visible;
		}

		/// <inheritdoc />
		public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new InvalidOperationException(this.GetType().Name + " cannot convert back a value to the source.");
		}

		#endregion
	}
}
