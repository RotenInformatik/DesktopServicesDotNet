using System;
using System.Collections;
using System.Collections.Generic;




namespace RI.DesktopServices.Settings.Storages
{
    /// <summary>
    ///     Implements a setting storage which reads from environment variables.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This setting store is read-only.
    ///     </para>
    ///     <para>
    ///         This setting store internally uses <see cref="Environment.GetEnvironmentVariables()" /> to read from the
    ///         current processes environment variables.
    ///     </para>
    ///     <para>
    ///         Because environment variables can be global and their names might be ambiguous, a prefix can be specified which
    ///         is then always appended in front of any name when searching for environment variables.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class EnvironmentVariableSettingStorage : DictionaryReadOnlySettingStorageBase
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
        public EnvironmentVariableSettingStorage ()
            : this(null) { }

        /// <summary>
        ///     Creates a new instance of <see cref="EnvironmentVariableSettingStorage" />.
        /// </summary>
        /// <param name="prefix"> The prefix to be used. </param>
        public EnvironmentVariableSettingStorage (string prefix)
        {
            prefix ??= string.Empty;
            this.Prefix = string.IsNullOrWhiteSpace(prefix) ? null : this.Prefix.Trim();
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the used prefix.
        /// </summary>
        /// <value>
        ///     The used prefix or null if no prefix is used.
        /// </value>
        public string Prefix { get; }

        #endregion




        #region Overrides

        /// <inheritdoc />
        protected override void ExtractValues (Dictionary<string, List<string>> dictionary)
        {
            IDictionary vars = Environment.GetEnvironmentVariables();

            foreach (DictionaryEntry vr in vars)
            {
                if (vr.Key is string key and not null)
                {
                    if (vr.Value is string value and not null)
                    {
                        if ((key.Length > 0) && (value.Length > 0) &&
                            ((this.Prefix == null) || key.StartsWith(this.Prefix,
                                                                     StringComparison.InvariantCultureIgnoreCase)))
                        {
                            if (!dictionary.ContainsKey(key))
                            {
                                dictionary.Add(key, new List<string>());
                            }

                            dictionary[key]
                                .Add(value);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
