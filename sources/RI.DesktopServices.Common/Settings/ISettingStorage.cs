using System;
using System.Collections.Generic;




namespace RI.DesktopServices.Settings
{
    /// <summary>
    ///     Defines the interface for a setting storage used by a setting service.
    /// </summary>
    public interface ISettingStorage
    {
        /// <summary>
        ///     Gets whether this setting storage only supports loading and retriving of values but not setting and saving.
        /// </summary>
        /// <value>
        ///     true if this setting storage only supports loading and retrieving.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         The default value should be false.
        ///     </note>
        /// </remarks>
        bool IsReadOnly { get; }

        /// <summary>
        ///     Gets whether the setting storage only writes/saves values for names it already has a value for.
        /// </summary>
        /// <value>
        ///     true if the setting storage only writes/saves values for names it already has a value for, false otherwise or if
        ///     this storage does not support writing/saving.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         The default value should be false.
        ///     </note>
        /// </remarks>
        bool WriteOnlyKnown { get; }

        /// <summary>
        ///     Gets the sequence which contains the prefix affinities of values when writing/saving.
        /// </summary>
        /// <value>
        ///     The sequence which contains the prefix affinities of values when writing/saving or null if this storage does not
        ///     support writing/saving.
        ///     The sequence is empty if prefix affinities are not used so all values of all names are written/saved (if
        ///     supported).
        /// </value>
        /// <remarks>
        ///     <para>
        ///         <see cref="WritePrefixAffinities" /> specifies zero, one, or more prefixes for value names.
        ///         Only values whose name starts with one of the specified prefixes are written/saved by this storage.
        ///     </para>
        /// </remarks>
        IReadOnlyList<string> WritePrefixAffinities { get; }

        /// <summary>
        ///     Deletes all values of a specified name.
        /// </summary>
        /// <param name="name"> The name of the value to delete. </param>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="ISettingService" />
        ///         implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="name" /> is an empty string. </exception>
        void DeleteValues (string name);

        /// <summary>
        ///     Deletes all values whose name matches a predicate.
        /// </summary>
        /// <param name="predicate"> The predicate used to test the names. </param>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="ISettingService" />
        ///         implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="predicate" /> is null. </exception>
        void DeleteValues (Predicate<string> predicate);

        /// <summary>
        ///     Reads the values of a specified name.
        /// </summary>
        /// <param name="name"> The name of the values. </param>
        /// <returns>
        ///     The values.
        ///     If no values are available, an empty list is returned.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="ISettingService" />
        ///         implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="name" /> is an empty string. </exception>
        List<string> GetValues (string name);

        /// <summary>
        ///     Reads the values based on a predicate which checks the names.
        /// </summary>
        /// <param name="predicate"> The predicate used to test the names. </param>
        /// <returns>
        ///     The values, arranged as a dictionary where the key is the name and the value is a list of actual setting values
        ///     belonging to the name.
        ///     An empty dictionary is returned if no values are found.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="ISettingService" />
        ///         implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="predicate" /> is null. </exception>
        Dictionary<string, List<string>> GetValues (Predicate<string> predicate);

        /// <summary>
        ///     Checks whether a value of a specified name is available.
        /// </summary>
        /// <param name="name"> The name of the value. </param>
        /// <returns>
        ///     true if the value is available, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="ISettingService" />
        ///         implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="name" /> is an empty string. </exception>
        bool HasValue (string name);

        /// <summary>
        ///     Determines whether a value is available whose name matches a predicate.
        /// </summary>
        /// <param name="predicate"> The predicate used to test the names. </param>
        /// <returns>
        ///     true if the value is available, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="ISettingService" />
        ///         implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="predicate" /> is null. </exception>
        bool HasValue (Predicate<string> predicate);

        /// <summary>
        ///     Reads, if necessary and applicable, all values from the storage.
        /// </summary>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="ISettingService" />
        ///         implementation.
        ///     </note>
        /// </remarks>
        void Load ();

        /// <summary>
        ///     Writes, if necessary and applicable, all values to the storage, making them persistent.
        /// </summary>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="ISettingService" />
        ///         implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="NotSupportedException"> The setting stoarge is read-only. </exception>
        void Save ();

        /// <summary>
        ///     Creates, deletes, or updates values of a specified name.
        /// </summary>
        /// <param name="name"> The name of the values. </param>
        /// <param name="values"> The actual values or null to delete all values. </param>
        /// <remarks>
        ///     <note type="important">
        ///         Any value of any name passed to <see cref="SetValues" /> must be written/saved by the storage!
        ///         Filtering based on <see cref="WriteOnlyKnown" /> and <see cref="WritePrefixAffinities" /> is done by
        ///         <see cref="ISettingService" /> and not the storage!
        ///     </note>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="ISettingService" />
        ///         implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="name" /> is an empty string. </exception>
        /// <exception cref="NotSupportedException"> The setting storage is read-only. </exception>
        void SetValues (string name, IEnumerable<string> values);
    }
}
