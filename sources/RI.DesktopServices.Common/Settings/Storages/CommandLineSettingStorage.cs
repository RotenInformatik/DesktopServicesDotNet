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
    ///         This setting store internally uses <see cref="Environment.GetCommandLineArgs" />
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class CommandLineSettingStorage : DictionaryReadOnlySettingStorageBase
    {
        #region Overrides

        /// <inheritdoc />
        protected override void ExtractValues (Dictionary<string, List<string>> dictionary)
        {
            string[] args = Environment.GetCommandLineArgs();

            for (int i1 = 1; i1 < args.Length; i1++)
            {
                string arg = args[i1];

                int index = arg.IndexOf('=');

                if (index > 0)
                {
                    string key = arg.Substring(0, index);
                    string value = arg.Substring(index + 1);

                    if ((key.Length > 0) && (value.Length > 0))
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

        #endregion
    }
}
