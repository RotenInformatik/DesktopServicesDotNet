using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Data;




namespace RI.Framework.Windows.Wpf.Converters
{
	/// <summary>
	///     Implements a converter between <see cref="object" /> and <see cref="bool" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="InvertedObjectBoolConverter" /> is used when a reference value of any type needs to be used as boolean value.
	///     </para>
	///     <para>
	///         Conversion from <see cref="object" /> (the source) to <see cref="bool" /> (the target):
	///         If the value is null, the converter returns true.
	///         If the value is not null, the converter returns false.
	///     </para>
	///     <para>
	///         Conversion from <see cref="bool" /> (the target) to <see cref="object" /> (the source):
	///         If the value is false, the converter returns the parameter.
	///         If the parameter is an <see cref="IEnumerable" />, its first element (or null if the sequence contains no elements) is returned.
	///         If the value is true, the converter returns null.
	///     </para>
	/// </remarks>
	[ValueConversion(typeof(object), typeof(bool), ParameterType = typeof(object))]
	public sealed class InvertedObjectBoolConverter : IValueConverter
	{
		#region Interface: IValueConverter

		/// <inheritdoc />
		public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value == null;
		}

		/// <inheritdoc />
		public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is bool))
			{
				return null;
			}

			return (bool)value ? null : (parameter is IEnumerable ? ((IEnumerable)parameter).Cast<object>().FirstOrDefault() : parameter);
		}

		#endregion
	}
}
