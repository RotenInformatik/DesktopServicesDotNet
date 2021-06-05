using System;
using System.Collections.Generic;




namespace RI.DesktopServices.Settings.Storages
{
    /// <summary>
    ///     Implements a setting storage which reads from the command line.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This setting store is read-only.
    ///     </para>
    ///     <para>
    ///         This setting store internally uses <see cref="RI.Framework.Utilities.Text.CommandLine.Parse(string,bool,IEqualityComparer{string})" /> with <see cref="Environment" />.<see cref="Environment.CommandLine" /> to read the current processes command line.
    ///         Only parameters, but not literals or the executable, is used for retriving values.
    ///     </para>
    ///     <para>
    ///         If a parameter occurs multiple times in the command line, only the first occurence is used.
    ///     </para>
    ///     <para>
    ///         See <see cref="ISettingStorage" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    public sealed class CommandLineSettingStorage : ISettingStorage
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="EnvironmentVariableSettingStorage" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         No prefix will be used.
        ///     </para>
        /// </remarks>
        public CommandLineSettingStorage ()
        {
            this.SyncRoot = new object();

            this.CommandLine = this.CommandLine.FromCurrentProcess(StringComparerEx.InvariantCultureIgnoreCase);
        }

        #endregion




        #region Instance Properties/Indexer

        private CommandLine CommandLine { get; set; }

        #endregion




        #region Interface: ISettingStorage

        /// <inheritdoc />
        bool ISettingStorage.IsReadOnly => true;

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        public object SyncRoot { get; }

        /// <inheritdoc />
        bool ISettingStorage.WriteOnlyKnown => false;

        /// <inheritdoc />
        IReadOnlyCollection<string> ISettingStorage.WritePrefixAffinities => null;

        /// <inheritdoc />
        public void DeleteValues (string name)
        {
            throw new NotSupportedException("Deleting a value from the command line is not supported.");
        }

        /// <inheritdoc />
        public void DeleteValues (Predicate<string> predicate)
        {
            throw new NotSupportedException("Deleting a value from the command line is not supported.");
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

            if (!this.CommandLine.Parameters.ContainsKey(name))
            {
                return new List<string>();
            }

            return this.CommandLine.Parameters[name].Where(x => x != null);
        }

        /// <inheritdoc />
        public Dictionary<string, List<string>> GetValues (Predicate<string> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            Dictionary<string, List<string>> values = new Dictionary<string, List<string>>(SettingService.NameComparer);
            Dictionary<string, List<string>> parameters = this.CommandLine.Parameters;
            foreach (KeyValuePair<string, List<string>> value in parameters)
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
        public bool HasValue (Predicate<string> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return this.CommandLine.Parameters.Any(x => predicate(x.Key) && (x.Value.Count > 0));
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

            if (!this.CommandLine.Parameters.ContainsKey(name))
            {
                return false;
            }

            if (this.CommandLine.Parameters[name].Count == 0)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public void Load ()
        {
        }

        /// <inheritdoc />
        public void Save ()
        {
            throw new NotSupportedException("Saving to the command line is not supported.");
        }

        /// <inheritdoc />
        public void SetValues (string name, IEnumerable<string> value)
        {
            throw new NotSupportedException("Setting a value to the command line is not supported.");
        }

        #endregion
    }
}
