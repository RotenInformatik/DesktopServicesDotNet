using System;
using System.Collections;
using System.Collections.Generic;

using RI.DesktopServices.Settings.Converters;
using RI.DesktopServices.Settings.Storages;




namespace RI.DesktopServices.Settings
{
    /// <summary>
    ///     Implements a default setting service which is suitable for most scenarios.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This setting service manages <see cref="ISettingStorage" />s  and <see cref="ISettingConverter" />s from two sources.
    ///         One are the explicitly specified storages and converters added through <see cref="AddStorage" /> and <see cref="AddConverter" />.
    ///         The second is a <see cref="CompositionContainer" /> if this <see cref="SettingService" /> is added as an export (the storages and converters are then imported through composition).
    ///         <see cref="Storages" /> gives the sequence containing all setting storages from all sources and <see cref="Converters" /> gives the sequence containing all setting converters from all sources.
    ///     </para>
    ///     <para>
    ///         See <see cref="ISettingService" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    public sealed class SettingService : LogSource, ISettingService, IImporting
    {
        #region Constants

        /// <summary>
        ///     Gets the string comparer which can be used to compare setting names.
        /// </summary>
        /// <value>
        ///     The string comparer which can be used to compare setting names.
        /// </value>
        public static readonly StringComparer NameComparer = StringComparerEx.InvariantCultureIgnoreCase;

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SettingService" />.
        /// </summary>
        public SettingService ()
        {
            this.SyncRoot = new object();

            this.StoragesUpdated = new List<ISettingStorage>();
            this.ConvertersUpdated = new List<ISettingConverter>();

            this.StoragesManual = new List<ISettingStorage>();
            this.ConvertersManual = new List<ISettingConverter>();

            this.Cache = new Dictionary<string, List<string>>(SettingService.NameComparer);
        }

        #endregion




        #region Instance Properties/Indexer

        private Dictionary<string, List<string>> Cache { get; set; }

        [Import(typeof(ISettingConverter), Recomposable = true)]
        private Import ConvertersImported { get; set; }

        private List<ISettingConverter> ConvertersManual { get; set; }

        private List<ISettingConverter> ConvertersUpdated { get; set; }

        [Import(typeof(ISettingStorage), Recomposable = true)]
        private Import StoragesImported { get; set; }

        private List<ISettingStorage> StoragesManual { get; set; }

        private List<ISettingStorage> StoragesUpdated { get; set; }

        #endregion




        #region Instance Methods

        private string ConvertFrom (ISettingConverter converter, Type type, object value)
        {
            Type usedType = this.GetConverterType(type);
            bool nullable = type.IsNullable();

            if (nullable && (value == null))
            {
                return string.Empty;
            }

            if (value == null)
            {
                return null;
            }

            return converter.ConvertFrom(usedType, value);
        }

        private object ConvertTo (ISettingConverter converter, Type type, string value)
        {
            Type usedType = this.GetConverterType(type);
            bool nullable = type.IsNullable();

            if (value == null)
            {
                return null;
            }

            if (nullable && value.IsEmpty())
            {
                return null;
            }

            return converter.ConvertTo(usedType, value);
        }

        private ISettingConverter GetConverterForType (Type type)
        {
            Type usedType = this.GetConverterType(type);

            foreach (ISettingConverter converter in this.Converters)
            {
                if (converter.ConversionMode != SettingConversionMode.StringConversion)
                {
                    continue;
                }

                if (converter.CanConvert(usedType))
                {
                    return converter;
                }
            }

            foreach (ISettingConverter converter in this.Converters)
            {
                if (converter.ConversionMode != SettingConversionMode.SerializationAsString)
                {
                    continue;
                }

                if (converter.CanConvert(usedType))
                {
                    return converter;
                }
            }

            return null;
        }

        private Type GetConverterType (Type type)
        {
            if (type.IsNullable())
            {
                return type.GetGenericArguments()[0];
            }

            return type;
        }

        private void UpdateConverters ()
        {
            this.Log(LogLevel.Debug, "Updating converters");

            HashSet<ISettingConverter> currentConverters = new HashSet<ISettingConverter>(this.Converters);
            HashSet<ISettingConverter> lastConverters = new HashSet<ISettingConverter>(this.ConvertersUpdated);

            HashSet<ISettingConverter> newConverters = currentConverters.Except(lastConverters);
            HashSet<ISettingConverter> oldConverters = lastConverters.Except(currentConverters);

            this.ConvertersUpdated.Clear();
            this.ConvertersUpdated.AddRange(currentConverters);

            foreach (ISettingConverter converter in newConverters)
            {
                this.Log(LogLevel.Debug, "Converter added: {0}", converter.GetType().Name);
            }

            foreach (ISettingConverter converter in oldConverters)
            {
                this.Log(LogLevel.Debug, "Converter removed: {0}", converter.GetType().Name);
            }
        }

        private void UpdateStorages ()
        {
            this.Log(LogLevel.Debug, "Updating storages");

            HashSet<ISettingStorage> currentStorages = new HashSet<ISettingStorage>(this.Storages);
            HashSet<ISettingStorage> lastStorages = new HashSet<ISettingStorage>(this.StoragesUpdated);

            HashSet<ISettingStorage> newStorages = currentStorages.Except(lastStorages);
            HashSet<ISettingStorage> oldStorages = lastStorages.Except(currentStorages);

            this.StoragesUpdated.Clear();
            this.StoragesUpdated.AddRange(currentStorages);

            foreach (ISettingStorage storage in newStorages)
            {
                this.Log(LogLevel.Debug, "Storage added: {0}", storage.GetType().Name);
            }

            foreach (ISettingStorage storage in oldStorages)
            {
                this.Log(LogLevel.Debug, "Storage removed: {0}", storage.GetType().Name);
            }
        }

        #endregion




        #region Interface: IImporting

        /// <inheritdoc />
        void IImporting.ImportsResolved (CompositionFlags composition, bool updated)
        {
            if (updated)
            {
                this.UpdateConverters();
                this.UpdateStorages();
            }
        }

        /// <inheritdoc />
        void IImporting.ImportsResolving (CompositionFlags composition)
        {
        }

        #endregion




        #region Interface: ISettingService

        /// <inheritdoc />
        public IEnumerable<ISettingConverter> Converters
        {
            get
            {
                foreach (ISettingConverter converter in this.ConvertersManual)
                {
                    yield return converter;
                }

                foreach (ISettingConverter converter in this.ConvertersImported.Values<ISettingConverter>())
                {
                    yield return converter;
                }
            }
        }

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        public IEnumerable<ISettingStorage> Storages
        {
            get
            {
                foreach (ISettingStorage storage in this.StoragesManual)
                {
                    yield return storage;
                }

                foreach (ISettingStorage storage in this.StoragesImported.Values<ISettingStorage>())
                {
                    yield return storage;
                }
            }
        }

        /// <inheritdoc />
        public object SyncRoot { get; }

        /// <inheritdoc />
        public void AddConverter (ISettingConverter settingConverter)
        {
            if (settingConverter == null)
            {
                throw new ArgumentNullException(nameof(settingConverter));
            }

            if (this.ConvertersManual.Contains(settingConverter))
            {
                return;
            }

            this.ConvertersManual.Add(settingConverter);

            this.UpdateConverters();
        }

        /// <inheritdoc />
        public void AddStorage (ISettingStorage settingStorage)
        {
            if (settingStorage == null)
            {
                throw new ArgumentNullException(nameof(settingStorage));
            }

            if (this.StoragesManual.Contains(settingStorage))
            {
                return;
            }

            this.StoragesManual.Add(settingStorage);

            this.UpdateStorages();
        }

        /// <inheritdoc />
        public void DeleteValues (string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            this.Cache.Remove(name);

            foreach (ISettingStorage store in this.Storages)
            {
                if (store.IsReadOnly)
                {
                    continue;
                }

                store.DeleteValues(name);
            }
        }

        /// <inheritdoc />
        public void DeleteValues (Predicate<string> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            this.Cache.RemoveWhere(x => predicate(x.Key));

            foreach (ISettingStorage store in this.Storages)
            {
                if (store.IsReadOnly)
                {
                    continue;
                }

                store.DeleteValues(predicate);
            }
        }

        /// <inheritdoc />
        public string GetRawValue (string name) => this.GetRawValues(name).FirstOrDefault();

        /// <inheritdoc />
        public List<string> GetRawValues (string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            if (this.Cache.ContainsKey(name))
            {
                return this.Cache[name].ToList();
            }

            List<string> finalValues = new List<string>();

            foreach (ISettingStorage store in this.Storages)
            {
                if (!store.IsReadOnly)
                {
                    continue;
                }

                List<string> values = store.GetValues(name);
                finalValues.AddRange(values);
            }

            foreach (ISettingStorage store in this.Storages)
            {
                if (store.IsReadOnly)
                {
                    continue;
                }

                List<string> values = store.GetValues(name);
                finalValues.AddRange(values);
            }

            return finalValues;
        }

        /// <inheritdoc />
        public Dictionary<string, List<string>> GetRawValues (Predicate<string> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            //We need to clear the cache, otherwise values might appear twice in the result
            this.Cache.Clear();

            Dictionary<string, List<string>> finalValues = new Dictionary<string, List<string>>(SettingService.NameComparer);
            Action<string, List<string>> addToFinalValues = (k, v) =>
            {
                if (!finalValues.ContainsKey(k))
                {
                    finalValues.Add(k, new List<string>());
                }

                finalValues[k].AddRange(v);
            };

            foreach (ISettingStorage store in this.Storages)
            {
                if (!store.IsReadOnly)
                {
                    continue;
                }

                Dictionary<string, List<string>> values = store.GetValues(predicate);
                values.ForEach(x => addToFinalValues(x.Key, x.Value));
            }

            foreach (ISettingStorage store in this.Storages)
            {
                if (store.IsReadOnly)
                {
                    continue;
                }

                Dictionary<string, List<string>> values = store.GetValues(predicate);
                values.ForEach(x => addToFinalValues(x.Key, x.Value));
            }

            return finalValues;
        }

        /// <inheritdoc />
        public T GetValue <T> (string name) => this.GetValues<T>(name).FirstOrDefault();

        /// <inheritdoc />
        public object GetValue (string name, Type type) => this.GetValues(name, type).FirstOrDefault();

        /// <inheritdoc />
        public List<T> GetValues <T> (string name) => this.GetValues(name, typeof(T)).Cast<T>();

        /// <inheritdoc />
        public List<object> GetValues (string name, Type type)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            ISettingConverter converter = this.GetConverterForType(type);
            if (converter == null)
            {
                throw new InvalidTypeArgumentException(nameof(type));
            }

            List<string> stringValues = this.GetRawValues(name);
            List<object> finalValues = stringValues.Select(x => this.ConvertTo(converter, type, x));
            return finalValues;
        }

        /// <inheritdoc />
        public Dictionary<string, List<T>> GetValues <T> (Predicate<string> predicate)
        {
            Dictionary<string, List<object>> values = this.GetValues(predicate, typeof(T));
            Dictionary<string, List<T>> finalValues = new Dictionary<string, List<T>>(SettingService.NameComparer);
            foreach (KeyValuePair<string, List<object>> value in values)
            {
                finalValues.Add(value.Key, new List<T>(value.Value.Select(x => (T)x)));
            }

            return finalValues;
        }

        /// <inheritdoc />
        public Dictionary<string, List<object>> GetValues (Predicate<string> predicate, Type type)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            ISettingConverter converter = this.GetConverterForType(type);
            if (converter == null)
            {
                throw new InvalidTypeArgumentException(nameof(type));
            }

            Dictionary<string, List<string>> stringValues = this.GetRawValues(predicate);
            Dictionary<string, List<object>> finalValues = new Dictionary<string, List<object>>(SettingService.NameComparer);
            foreach (KeyValuePair<string, List<string>> stringValue in stringValues)
            {
                finalValues.Add(stringValue.Key, new List<object>(stringValue.Value.Select(x => this.ConvertTo(converter, type, x))));
            }

            return finalValues;
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

            if (this.Cache.ContainsKey(name))
            {
                return true;
            }

            foreach (ISettingStorage store in this.Storages)
            {
                if (store.HasValue(name))
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public bool HasValue (Predicate<string> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (this.Cache.Any(x => predicate(x.Key)))
            {
                return true;
            }

            foreach (ISettingStorage store in this.Storages)
            {
                if (store.HasValue(predicate))
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public bool InitializeRawValue (string name, string defaultValue) => this.InitializeRawValues(name, new[] {defaultValue});

        /// <inheritdoc />
        public bool InitializeRawValues (string name, IEnumerable<string> defaultValues)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            List<string> finalValues = defaultValues?.ToList() ?? new List<string>();
            if (finalValues.Count == 0)
            {
                return false;
            }

            if (this.HasValue(name))
            {
                return false;
            }

            this.SetRawValues(name, finalValues);

            return true;
        }

        /// <inheritdoc />
        public bool InitializeValue <T> (string name, T defaultValue) => this.InitializeValue(name, defaultValue, typeof(T));

        /// <inheritdoc />
        public bool InitializeValue (string name, object defaultValue, Type type) => this.InitializeValues(name, new[] {defaultValue}, type);

        /// <inheritdoc />
        public bool InitializeValues <T> (string name, IEnumerable<T> defaultValues) => this.InitializeValues(name, defaultValues, typeof(T));

        /// <inheritdoc />
        public bool InitializeValues (string name, IEnumerable defaultValues, Type type)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            ISettingConverter converter = this.GetConverterForType(type);
            if (converter == null)
            {
                throw new InvalidTypeArgumentException(nameof(type));
            }

            if (defaultValues == null)
            {
                return false;
            }

            List<string> finalValues = defaultValues.Cast<object>().Select(x => this.ConvertFrom(converter, type, x));
            if (finalValues.Count == 0)
            {
                return false;
            }

            return this.InitializeRawValues(name, finalValues);
        }

        /// <inheritdoc />
        public void Load ()
        {
            this.Log(LogLevel.Debug, "Loading");

            this.Cache.Clear();

            foreach (ISettingStorage store in this.Storages)
            {
                store.Load();
            }
        }

        /// <inheritdoc />
        public void RemoveConverter (ISettingConverter settingConverter)
        {
            if (settingConverter == null)
            {
                throw new ArgumentNullException(nameof(settingConverter));
            }

            if (!this.ConvertersManual.Contains(settingConverter))
            {
                return;
            }

            this.ConvertersManual.RemoveAll(settingConverter);

            this.UpdateConverters();
        }

        /// <inheritdoc />
        public void RemoveStorage (ISettingStorage settingStorage)
        {
            if (settingStorage == null)
            {
                throw new ArgumentNullException(nameof(settingStorage));
            }

            if (!this.StoragesManual.Contains(settingStorage))
            {
                return;
            }

            this.StoragesManual.RemoveAll(settingStorage);

            this.UpdateStorages();
        }

        /// <inheritdoc />
        public void Save ()
        {
            this.Log(LogLevel.Debug, "Saving");

            this.Cache.Clear();

            foreach (ISettingStorage store in this.Storages)
            {
                if (store.IsReadOnly)
                {
                    continue;
                }

                store.Save();
            }
        }

        /// <inheritdoc />
        public void SetRawValue (string name, string value) => this.SetRawValues(name, new[] {value});

        /// <inheritdoc />
        public void SetRawValues (string name, IEnumerable<string> values)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            List<string> finalValues = values?.ToList() ?? new List<string>();
            if (finalValues.Count == 0)
            {
                this.DeleteValues(name);
                return;
            }

            this.Cache.Remove(name);
            if (finalValues.Count > 0)
            {
                this.Cache.Add(name, finalValues);
            }

            int stores = 0;

            foreach (ISettingStorage store in this.Storages)
            {
                if (store.IsReadOnly)
                {
                    continue;
                }

                if (store.WriteOnlyKnown && (!store.HasValue(name)))
                {
                    continue;
                }

                if ((store.WritePrefixAffinities != null) && (store.WritePrefixAffinities.Count > 0) && (!store.WritePrefixAffinities.Any(x => name.StartsWith(x, StringComparison.InvariantCultureIgnoreCase))))
                {
                    continue;
                }

                store.SetValues(name, finalValues);

                stores++;
            }

            if (stores == 0)
            {
                this.Log(LogLevel.Warning, "Setting {0} not written to any storage", name);
            }
        }

        /// <inheritdoc />
        public void SetValue <T> (string name, T value) => this.SetValue(name, value, typeof(T));

        /// <inheritdoc />
        public void SetValue (string name, object value, Type type) => this.SetValues(name, new[] {value}, type);

        /// <inheritdoc />
        public void SetValues <T> (string name, IEnumerable<T> values) => this.SetValues(name, values, typeof(T));

        /// <inheritdoc />
        public void SetValues (string name, IEnumerable values, Type type)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            ISettingConverter converter = this.GetConverterForType(type);
            if (converter == null)
            {
                throw new InvalidTypeArgumentException(nameof(type));
            }

            if (values == null)
            {
                this.DeleteValues(name);
                return;
            }

            List<string> finalValues = values.Cast<object>().Select(x => this.ConvertFrom(converter, type, x));
            if (finalValues.Count == 0)
            {
                this.DeleteValues(name);
                return;
            }

            this.SetRawValues(name, finalValues);
        }

        #endregion
    }
}
