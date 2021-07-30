using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;




namespace RI.DesktopServices.Settings.Storages
{
    /// <summary>
    ///     Implements a setting storage which reads/writes from/to INI files.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This setting store is not read-only.
    ///     </para>
    ///     <para>
    ///         Sections in INI files are not supported. When reading, section headers are ignored. When writing, the file is
    ///         emptied before the values are written without any sections (except <see cref="WriteOnlyKnown" /> is set, in
    ///         which cases the structure of the file is preserved).
    ///     </para>
    ///     <para>
    ///         If the specified INI file does not exist, it will be created and no values will be read from it (because its
    ///         empty after creation...).
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class IniFileSettingStorage : DictionaryStorageBase
    {
        #region Static Fields

        /// <summary>
        ///     The default text encoding which is used for INI files.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The default text encoding is UTF-8.
        ///     </para>
        /// </remarks>
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="IniFileSettingStorage" />.
        /// </summary>
        /// <param name="filePath"> The path to the INI file. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="filePath" /> is an empty string or contains an invalid path. </exception>
        /// <remarks>
        ///     <para>
        ///         The default encoding <see cref="DefaultEncoding" /> is used as the text encoding for the INI file.
        ///     </para>
        ///     <para>
        ///         All values will be written, not only those known (see <see cref="WriteOnlyKnown" />).
        ///     </para>
        ///     <para>
        ///         No write prefix affinities will be used (see <see cref="WritePrefixAffinities" />).
        ///     </para>
        /// </remarks>
        public IniFileSettingStorage (string filePath)
            : this(filePath, null, false, (IEnumerable<string>)null) { }

        /// <summary>
        ///     Creates a new instance of <see cref="IniFileSettingStorage" />.
        /// </summary>
        /// <param name="filePath"> The path to the INI file. </param>
        /// <param name="fileEncoding"> The text encoding of the INI file (can be null to use <see cref="DefaultEncoding" />). </param>
        /// <param name="writeOnlyKnown">
        ///     Specifies whether the setting storage only writes/saves values for names it already has a
        ///     value for.
        /// </param>
        /// <param name="writePrefixAffinities">
        ///     A sequence of prefix affinities of values when writing/saving or null if this
        ///     storage should not use any.
        /// </param>
        /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="filePath" /> is an empty string or contains an invalid path. </exception>
        public IniFileSettingStorage (string filePath, Encoding fileEncoding, bool writeOnlyKnown,
                                      IEnumerable<string> writePrefixAffinities)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("The string is empty.", nameof(filePath));
            }

            this.FilePath = filePath;
            this.FileEncoding = fileEncoding ?? IniFileSettingStorage.DefaultEncoding;
            this.WriteOnlyKnownInternal = writeOnlyKnown;
            this.WritePrefixAffinitiesInternal = new List<string>(writePrefixAffinities ?? new string[0]);
        }

        /// <summary>
        ///     Creates a new instance of <see cref="IniFileSettingStorage" />.
        /// </summary>
        /// <param name="filePath"> The path to the INI file. </param>
        /// <param name="fileEncoding"> The text encoding of the INI file (can be null to use <see cref="DefaultEncoding" />). </param>
        /// <param name="writeOnlyKnown">
        ///     Specifies whether the setting storage only writes/saves values for names it already has a
        ///     value for.
        /// </param>
        /// <param name="writePrefixAffinities">
        ///     A sequence of prefix affinities of values when writing/saving or none/null if this
        ///     storage should not use any.
        /// </param>
        /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="filePath" /> is an empty string or contains an invalid path. </exception>
        public IniFileSettingStorage (string filePath, Encoding fileEncoding, bool writeOnlyKnown,
                                      params string[] writePrefixAffinities)
            : this(filePath, fileEncoding, writeOnlyKnown, (IEnumerable<string>)writePrefixAffinities) { }

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
        public string FilePath { get; }

        private bool WriteOnlyKnownInternal { get; }

        private IReadOnlyList<string> WritePrefixAffinitiesInternal { get; }

        #endregion




        #region Instance Methods

        private List<(string key, string value)> ExtractPairsFromLines (StreamReader reader)
        {
            List<(string key, string value)> pairs = new List<(string key, string value)>();

            while (true)
            {
                string line = reader.ReadLine();

                if (line == null)
                {
                    break;
                }

                int index = line.IndexOf('=');

                bool useWholeLine = true;

                if (index > 0)
                {
                    string key = line.Substring(0, index);
                    string value = line.Substring(index + 1);

                    if ((key.Length > 0) && (value.Length > 0))
                    {
                        pairs.Add((key, value));
                        useWholeLine = false;
                    }
                }

                if (useWholeLine)
                {
                    pairs.Add((null, line));
                }
            }

            return pairs;
        }

        private string MakeLineFromPair ((string key, string value) pair)
        {
            if (pair.key == null)
            {
                return pair.value;
            }

            return $"{pair.key}={pair.value}";
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override bool WriteOnlyKnown => this.WriteOnlyKnownInternal;

        /// <inheritdoc />
        public override IReadOnlyList<string> WritePrefixAffinities => this.WritePrefixAffinitiesInternal;

        /// <inheritdoc />
        protected override void LoadValues (Dictionary<string, List<string>> dictionary)
        {
            if (!File.Exists(this.FilePath))
            {
                Trace.TraceWarning($"INI settings file does not exist, an empty file will be created: {this.FilePath}.");
            }

            Trace.TraceInformation($"Loading INI settings file: {this.FilePath}.");

            using FileStream fs = File.Open(this.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            using StreamReader sr = new StreamReader(fs, this.FileEncoding);

            List<(string key, string value)> pairs = this.ExtractPairsFromLines(sr);

            foreach ((string key, string value) pair in pairs)
            {
                if (string.IsNullOrWhiteSpace(pair.key))
                {
                    continue;
                }

                if (!dictionary.ContainsKey(pair.key))
                {
                    dictionary.Add(pair.key, new List<string>());
                }

                dictionary[pair.key]
                    .Add(pair.value);
            }
        }

        /// <inheritdoc />
        protected override void SaveValues (Dictionary<string, List<string>> dictionary)
        {
            if (!File.Exists(this.FilePath))
            {
                Trace.TraceWarning($"INI settings file does not exist, an new file will be created: {this.FilePath}.");
            }

            Trace.TraceInformation($"Saving INI settings file: {this.FilePath}.");

            List<(string key, string value)> newLines = new List<(string key, string value)>();

            if (this.WriteOnlyKnown)
            {
                using FileStream fsRead =
                    File.Open(this.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

                using StreamReader sr = new StreamReader(fsRead, this.FileEncoding);

                List<(string key, string value)> existingLines = this.ExtractPairsFromLines(sr);

                sr.Close();
                fsRead.Close();

                for (int i1 = 0; i1 < existingLines.Count; i1++)
                {
                    (string key, string value) existingLine = existingLines[i1];

                    if ((existingLine.key != null) && dictionary.ContainsKey(existingLine.key))
                    {
                        for (int i2 = 0; i2 < dictionary[existingLine.key]
                                             .Count; i2++)
                        {
                            string value = dictionary[existingLine.key][i2];
                            newLines.Add((existingLine.key, value));
                        }
                    }
                    else
                    {
                        newLines.Add(existingLine);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, List<string>> setting in dictionary)
                {
                    foreach (string value in setting.Value)
                    {
                        string newLine = this.MakeLineFromPair((setting.Key, value));
                        newLines.Add((setting.Key, newLine));
                    }
                }
            }

            using FileStream fsWrite = File.Open(this.FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            using StreamWriter sw = new StreamWriter(fsWrite, this.FileEncoding);

            foreach ((string key, string value) pair in newLines)
            {
                sw.WriteLine(pair.value);
            }
        }

        #endregion
    }
}
