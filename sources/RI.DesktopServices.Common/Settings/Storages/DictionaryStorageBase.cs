using System;
using System.Collections.Generic;




namespace RI.DesktopServices.Settings.Storages
{
    /// <summary>
    ///     Boilerplate implementation for <see cref="ISettingStorage" /> which is not read-only and based on using a
    ///     dictionary internally.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public abstract class DictionaryStorageBase : DictionaryReadOnlySettingStorageBase
    {
        #region Overrides

        /// <inheritdoc />
        public override bool IsReadOnly => false;

        /// <inheritdoc />
        public override void DeleteValues (string name)
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
            values.Remove(name);
        }

        /// <inheritdoc />
        public override void DeleteValues (Predicate<string> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            Dictionary<string, List<string>> values = this.GetExtractedValues();
            List<string> namesToDelete = new List<string>();

            foreach (KeyValuePair<string, List<string>> value in values)
            {
                if (predicate(value.Key))
                {
                    namesToDelete.Add(value.Key);
                }
            }

            foreach (string nameToDelete in namesToDelete)
            {
                values.Remove(nameToDelete);
            }
        }

        /// <inheritdoc />
        public override void Load ()
        {
            Dictionary<string, List<string>> values = this.GetExtractedValues();
            values.Clear();
            this.LoadValues(values);
        }

        /// <inheritdoc />
        public override void Save ()
        {
            Dictionary<string, List<string>> values = this.GetExtractedValues();
            this.SaveValues(values);
        }

        /// <inheritdoc />
        public override void SetValues (string name, IEnumerable<string> values)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The string is empty.", nameof(name));
            }

            Dictionary<string, List<string>> dictionary = this.GetExtractedValues();
            dictionary.Remove(name);
            dictionary.Add(name, new List<string>(values ?? new string[0]));
        }

        /// <inheritdoc />
        protected override void ExtractValues (Dictionary<string, List<string>> dictionary) { }

        #endregion




        #region Abstracts

        /// <summary>
        ///     Loads the values specific to this setting storage into a dictionary.
        /// </summary>
        /// <param name="dictionary"> The dictionary of values to load into. </param>
        protected abstract void LoadValues (Dictionary<string, List<string>> dictionary);

        /// <summary>
        ///     Saves the values specific to this setting storage from a dictionary.
        /// </summary>
        /// <param name="dictionary"> The dictionary of values to save from. </param>
        protected abstract void SaveValues (Dictionary<string, List<string>> dictionary);

        #endregion
    }
}
