using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Windows.Wpf.Converters
{
	/// <summary>
	///     Implements a converter between <see cref="object" /> and <see cref="Visibility" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="NormalObjectVisibilityConverter" /> is used when a reference value of any type needs to be used as the visibility for a WPF element.
	///         If the value is not null, the converter returns <see cref="Visibility.Visible" />.
	///         If the value is null, the converter returns its parameter of type <see cref="Visibility" />.
	///         This allows to choose whether <see cref="Visibility.Hidden" /> or <see cref="Visibility.Collapsed" /> is used when the value is null.
	///     </para>
	///     <para>
	///         The converter only supports conversion from <see cref="object" /> (the source) to <see cref="Visibility" /> (the target).
	///     </para>
	/// </remarks>
	[ValueConversion(typeof(object), typeof(Visibility), ParameterType = typeof(Visibility))]
	public sealed class NormalObjectVisibilityConverter : IValueConverter
	{
		#region Interface: IValueConverter

		/// <inheritdoc />
		public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(parameter is Visibility))
			{
				throw new InvalidTypeArgumentException(nameof(parameter));
			}

			return (value != null) ? Visibility.Visible : (Visibility)parameter;
		}

		/// <inheritdoc />
		public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new InvalidOperationException(this.GetType().Name + " cannot convert back a value to the source.");
		}

		#endregion
	}
}
