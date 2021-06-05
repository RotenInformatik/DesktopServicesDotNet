using System;

using RI.DesktopServices.Settings.Converters;




namespace RI.DesktopServices.Settings
{
    /// <summary>
    ///     Defines the interface for a setting converter used by a setting service.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A setting converter is used by a <see cref="ISettingService" /> to convert between setting values and their string representation for use in the underlying storage.
    ///     </para>
    ///     <note type="implement">
    ///         A setting converter must always support symmetrical conversion, meaning that conversion must always be possible in both directions.
    ///     </note>
    /// </remarks>
    public interface ISettingConverter
    {
        /// <summary>
        ///     Gets the used setting conversion mode
        /// </summary>
        /// <value>
        ///     The used setting conversion mode.
        /// </value>
        SettingConversionMode ConversionMode { get; }

        /// <summary>
        ///     Determines whether this converter supports conversion to and from the specified setting type.
        /// </summary>
        /// <param name="type"> The setting type. </param>
        /// <returns>
        ///     true if this converter can convert to and from the type specified by <paramref name="type" />, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="ISettingService" /> implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
        bool CanConvert (Type type);

        /// <summary>
        ///     Converts a setting value from the specified type to its string representation.
        /// </summary>
        /// <param name="type"> The setting type. </param>
        /// <param name="value"> The setting value. </param>
        /// <returns>
        ///     The string representation of the setting value as converted by this converter or null if <paramref name="value" /> is null.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="ISettingService" /> implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="type" /> or <paramref name="value" /> is null. </exception>
        /// <exception cref="InvalidTypeArgumentException"> The specified <paramref name="type" /> or the type of <paramref name="value" /> is not supported by this converter or <paramref name="type" /> and <paramref name="value" /> do not match. </exception>
        string ConvertFrom (Type type, object value);

        /// <summary>
        ///     Converts a setting value from its string representation to the specified type.
        /// </summary>
        /// <param name="type"> The setting type. </param>
        /// <param name="value"> The string representation of the setting value. </param>
        /// <returns>
        ///     The setting value in the specified type as converted by this converter or null if <paramref name="value" /> is null..
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="ISettingService" /> implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
        /// <exception cref="InvalidTypeArgumentException"> The specified <paramref name="type" /> is not supported by this converter. </exception>
        /// <exception cref="FormatException"> The string representation of <paramref name="value" /> is invalid and cannot be converted to <paramref name="type" />. </exception>
        object ConvertTo (Type type, string value);
    }
}
