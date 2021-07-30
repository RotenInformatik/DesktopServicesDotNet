using System;
using System.Collections.Generic;




namespace RI.DesktopServices.Settings.Storages
{
    /// <summary>
    ///     Boilerplate implementation for <see cref="ISettingStorage" /> which is read-only and based on using a dictionary
    ///     internally.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public abstract class DictionaryReadOnlySettingStorageBase : ISettingStorage
    {
        #region Instance Properties/Indexer

        private Dictionary<string, List<string>> Values { get; set; }

        #endregion




        #region Virtuals

        /// <summary>
        ///     Gets the dictionary of all extracted values.
        /// </summary>
        /// <returns>
        ///     The dictionary of all extracted values.
        /// </returns>
        protected virtual Dictionary<string, List<string>> GetExtractedValues ()
        {
            if (this.Values == null)
            {
                this.Values = new Dictionary<string, List<string>>(SettingService.NameComparer);
                this.ExtractValues(this.Values);
            }

            return this.Values;
        }

        #endregion




        #region Abstracts

        /// <summary>
        ///     Extracts the values specific to this setting storage into a dictionary.
        /// </summary>
        /// <param name="dictionary"> The dictionary to fill with the extracted values. </param>
        protected abstract void ExtractValues (Dictionary<string, List<string>> dictionary);

        #endregion




        #region Interface: ISettingStorage

        /// <inheritdoc />
        public virtual bool IsReadOnly => true;

        /// <inheritdoc />
        public virtual bool WriteOnlyKnown => false;

        /// <inheritdoc />
        public virtual IReadOnlyList<string> WritePrefixAffinities => null;

        /// <inheritdoc />
        public virtual void DeleteValues (string name) =>
            throw new
                NotSupportedException("Deleting values from the command line is not supported by {this.GetType().Name}.");

        /// <inheritdoc />
        public virtual void DeleteValues (Predicate<string> predicate) =>
            throw new
                NotSupportedException("Deleting values from the command line is not supported by {this.GetType().Name}.");

        /// <inheritdoc />
        public virtual List<string> GetValues (string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The string is empty.", nameof(name));
            }

            Dictionary<string, List<string>> values = this.GetExtractedValues();
            List<string> result = values.ContainsKey(name) ? values[name] : new List<string>();
            return result;
        }

        /// <inheritdoc />
        public virtual Dictionary<string, List<string>> GetValues (Predicate<string> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            Dictionary<string, List<string>> values = this.GetExtractedValues();
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>(values.Comparer);

            foreach (KeyValuePair<string, List<string>> value in values)
            {
                if (predicate(value.Key))
                {
                    result.Add(value.Key, value.Value ?? new List<string>());
                }
            }

            return result;
        }

        /// <inheritdoc />
        public virtual bool HasValue (Predicate<string> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            Dictionary<string, List<string>> values = this.GetExtractedValues();

            foreach (KeyValuePair<string, List<string>> value in values)
            {
                if (predicate(value.Key))
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public virtual bool HasValue (string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The string is empty.", nameof(name));
            }

            Dictionary<string, List<string>> values = this.GetExtractedValues();

            bool result = (values.ContainsKey(name) && (values[name] != null) && (values[name]
                                       .Count > 0));

            return result;
        }

        /// <inheritdoc />
        public virtual void Load () { }

        /// <inheritdoc />
        public virtual void Save () =>
            throw new
                NotSupportedException($"Saving values to the command line is not supported by {this.GetType().Name}.");

        /// <inheritdoc />
        public virtual void SetValues (string name, IEnumerable<string> values) =>
            throw new
                NotSupportedException($"Writing values to the command line is not supported by {this.GetType().Name}.");

        #endregion
    }
}
