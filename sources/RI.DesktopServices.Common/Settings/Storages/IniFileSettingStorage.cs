using System;
using System.Collections.Generic;
using System.Text;




namespace RI.DesktopServices.Settings.Storages
{
    /// <summary>
    ///     Implements a setting storage which reads/writes from/to a specified INI file.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This setting store internally uses <see cref="IniDocument" /> to process the INI file.
    ///         See <see cref="IniDocument" /> for more details about INI files.
    ///     </para>
    ///     <para>
    ///         See <see cref="ISettingStorage" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    public sealed class IniFileSettingStorage : LogSource, ISettingStorage
    {
        #region Constants

        /// <summary>
        ///     The default text encoding which is used for INI files.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The default text encoding is UTF-8.
        ///     </para>
        /// </remarks>
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        /// <summary>
        ///     The default INI section name for reading/writing values.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The default file name is null, which searches and places all values outside any section.
        ///     </para>
        /// </remarks>
        public static readonly string DefaultSectionName = null;

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="IniFileSettingStorage" />.
        /// </summary>
        /// <param name="filePath"> The path to the INI file. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is null. </exception>
        /// <exception cref="InvalidPathArgumentException"> <paramref name="filePath" /> contains wildcards. </exception>
        /// <remarks>
        ///     <para>
        ///         The default encoding <see cref="DefaultEncoding" /> is used as the text encoding for the INI file.
        ///     </para>
        ///     <para>
        ///         The default section name <see cref="DefaultSectionName" /> is used as the INI section name.
        ///     </para>
        ///     <para>
        ///         All values will be written, not only those known (see <see cref="WriteOnlyKnown" />).
        ///     </para>
        ///     <para>
        ///         No write prefix affinities will be used (see <see cref="WritePrefixAffinities" />).
        ///     </para>
        /// </remarks>
        public IniFileSettingStorage (FilePath filePath)
            : this(filePath, null, null, false, (IEnumerable<string>)null)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="IniFileSettingStorage" />.
        /// </summary>
        /// <param name="filePath"> The path to the INI file. </param>
        /// <param name="fileEncoding"> The text encoding of the INI file (can be null to use <see cref="DefaultEncoding" />). </param>
        /// <param name="sectionName"> The INI section name where all the values are read/written from/to (can be null to use <see cref="DefaultSectionName" />). </param>
        /// <param name="writeOnlyKnown"> Specifies whether the setting storage only writes/saves values for names it already has a value for. </param>
        /// <param name="writePrefixAffinities"> A sequence of prefix affinities of values when writing/saving or null if this storage should not use any. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is null. </exception>
        /// <exception cref="InvalidPathArgumentException"> <paramref name="filePath" /> contains wildcards. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="sectionName" /> is an empty string. </exception>
        public IniFileSettingStorage (FilePath filePath, Encoding fileEncoding, string sectionName, bool writeOnlyKnown, IEnumerable<string> writePrefixAffinities)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (filePath.HasWildcards)
            {
                throw new InvalidPathArgumentException(nameof(filePath));
            }

            if (sectionName != null)
            {
                if (sectionName.IsEmptyOrWhitespace())
                {
                    throw new EmptyStringArgumentException(nameof(sectionName));
                }
            }

            this.SyncRoot = new object();
            this.FilePath = filePath;
            this.FileEncoding = fileEncoding ?? IniFileSettingStorage.DefaultEncoding;
            this.SectionName = sectionName ?? IniFileSettingStorage.DefaultSectionName;
            this.WriteOnlyKnown = writeOnlyKnown;
            this.WritePrefixAffinities = new HashSet<string>(writePrefixAffinities ?? new string[0], SettingService.NameComparer);
            this.Document = new IniDocument(StringComparerEx.InvariantCultureIgnoreCase);
        }

        /// <summary>
        ///     Creates a new instance of <see cref="IniFileSettingStorage" />.
        /// </summary>
        /// <param name="filePath"> The path to the INI file. </param>
        /// <param name="fileEncoding"> The text encoding of the INI file (can be null to use <see cref="DefaultEncoding" />). </param>
        /// <param name="sectionName"> The INI section name where all the values are read/written from/to (can be null to use <see cref="DefaultSectionName" />). </param>
        /// <param name="writeOnlyKnown"> Specifies whether the setting storage only writes/saves values for names it already has a value for. </param>
        /// <param name="writePrefixAffinities"> An array of prefix affinities of values when writing/saving or null if this storage should not use any. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is null. </exception>
        /// <exception cref="InvalidPathArgumentException"> <paramref name="filePath" /> contains wildcards. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="sectionName" /> is an empty string. </exception>
        public IniFileSettingStorage (FilePath filePath, Encoding fileEncoding, string sectionName, bool writeOnlyKnown, params string[] writePrefixAffinities)
            : this(filePath, fileEncoding, sectionName, writeOnlyKnown, (IEnumerable<string>)writePrefixAffinities)
        {
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the text encoding of the used INI file.
        /// </summary>
        /// <value>
        ///     The text encoding of the used INI file.
        /// </value>
        public Encoding FileEncoding { get; }

        /// <summary>
        ///     Gets the path to the used INI file.
        /// </summary>
        /// <value>
        ///     The path to the used INI file.
        /// </value>
        public FilePath FilePath { get; }

        /// <summary>
        ///     Gets the INI section name where all the values are read/written from/to in the INI file.
        /// </summary>
        /// <value>
        ///     The INI section name where all the values are read/written from/to in the INI file.
        /// </value>
        public string SectionName { get; }

        private IniDocument Document { get; set; }

        #endregion




        #region Interface: ISettingStorage

        /// <inheritdoc />
        bool ISettingStorage.IsReadOnly => false;

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        public object SyncRoot { get; }

        /// <inheritdoc />
        public bool WriteOnlyKnown { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> WritePrefixAffinities { get; }

        /// <inheritdoc />
        public void DeleteValues (string name) => this.SetValues(name, null);

        /// <inheritdoc />
        public void DeleteValues (Predicate<string> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            Dictionary<string, string> section = this.Document.GetSection(this.SectionName);
            List<string> namesToRemove = section.Select(x => x.Key).Where(x => predicate(x));
            namesToRemove.ForEach(this.DeleteValues);
        }

        /// <inheritdoc />
        public List<string> GetValues (string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            List<string> values = this.Document.GetValueAll(this.SectionName, name).Where(x => x != null);
            return values;
        }

        /// <inheritdoc />
        public Dictionary<string, List<string>> GetValues (Predicate<string> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            Dictionary<string, List<string>> values = new Dictionary<string, List<string>>(SettingService.NameComparer);
            Dictionary<string, List<string>> section = this.Document.GetSectionAll(this.SectionName);
            foreach (KeyValuePair<string, List<string>> value in section)
            {
                if (predicate(value.Key))
                {
                    if (!values.ContainsKey(value.Key))
                    {
                        values.Add(value.Key, new List<string>());
                    }

                    values[value.Key].AddRange(value.Value);
                }
            }

            return values;
        }

        /// <inheritdoc />
        public bool HasValue (string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            return this.Document.GetValue(this.SectionName, name) != null;
        }

        /// <inheritdoc />
        public bool HasValue (Predicate<string> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            Dictionary<string, string> section = this.Document.GetSection(this.SectionName);
            return section.Any(x => predicate(x.Key));
        }

        /// <inheritdoc />
        public void Load ()
        {
            if (!this.FilePath.Exists)
            {
                this.Log(LogLevel.Debug, "Creating INI settings file: {0}", this.FilePath);
            }

            this.FilePath.Directory.Create();
            this.FilePath.CreateIfNotExist();

            this.Log(LogLevel.Debug, "Loading INI settings file: {0}", this.FilePath);

            this.Document.Clear();
            this.Document.Load(this.FilePath, this.FileEncoding);
        }

        /// <inheritdoc />
        public void Save ()
        {
            if (!this.FilePath.Exists)
            {
                this.Log(LogLevel.Debug, "Creating INI settings file: {0}", this.FilePath);
            }

            this.FilePath.Directory.Create();

            this.Log(LogLevel.Debug, "Saving INI settings file: {0}", this.FilePath);

            this.Document.SortElements(this.SectionName);
            this.Document.Save(this.FilePath, this.FileEncoding);
        }

        /// <inheritdoc />
        public void SetValues (string name, IEnumerable<string> value)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            List<string> finalValues = value?.ToList() ?? new List<string>();

            this.Document.SetValueAll(this.SectionName, name, finalValues);
        }

        #endregion
    }
}
