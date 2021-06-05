using System;
using System.Collections.Generic;




namespace RI.DesktopServices.Settings
{
    /// <summary>
    ///     Implements simplified access to a setting value by combining setting name, default values, and accessor functions.
    /// </summary>
    /// <typeparam name="T"> The type of the setting. </typeparam>
    /// <threadsafety static="false" instance="false" />
    public sealed class SettingItem <T>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SettingItem{T}" />.
        /// </summary>
        /// <param name="name"> The setting name. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        public SettingItem (string name)
            : this(name, null, (IEnumerable<T>)null)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SettingItem{T}" />.
        /// </summary>
        /// <param name="name"> </param>
        /// <param name="serviceResolver"> The service resolver used to get the setting service. Can be null to provide the service directly to the accessor functions or to use <see cref="ServiceLocator" />. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        public SettingItem (string name, Func<ISettingService> serviceResolver)
            : this(name, serviceResolver, (IEnumerable<T>)null)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SettingItem{T}" />.
        /// </summary>
        /// <param name="name"> The setting name. </param>
        /// <param name="defaultValues"> The default values. Can be null or empty. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        public SettingItem (string name, params T[] defaultValues)
            : this(name, null, (IEnumerable<T>)defaultValues)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SettingItem{T}" />.
        /// </summary>
        /// <param name="name"> The setting name. </param>
        /// <param name="defaultValues"> The default values. Can be null or empty. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="defaultValues" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        public SettingItem (string name, IEnumerable<T> defaultValues)
            : this(name, null, defaultValues)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SettingItem{T}" />.
        /// </summary>
        /// <param name="name"> The setting name. </param>
        /// <param name="serviceResolver"> The service resolver used to get the setting service. Can be null to provide the service directly to the accessor functions or to use <see cref="ServiceLocator" />. </param>
        /// <param name="defaultValues"> The default values. Can be null or empty. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        public SettingItem (string name, Func<ISettingService> serviceResolver, params T[] defaultValues)
            : this(name, serviceResolver, (IEnumerable<T>)defaultValues)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SettingItem{T}" />.
        /// </summary>
        /// <param name="name"> The setting name. </param>
        /// <param name="serviceResolver"> The service resolver used to get the setting service. Can be null to provide the service directly to the accessor functions or to use <see cref="ServiceLocator" />. </param>
        /// <param name="defaultValues"> The default values. Can be null or empty. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="defaultValues" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        public SettingItem (string name, Func<ISettingService> serviceResolver, IEnumerable<T> defaultValues)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsNullOrWhitespaces())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            this.Name = name;
            this.DefaultValues = new List<T>(defaultValues ?? new T[0]);
            this.ServiceResolver = serviceResolver;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the list of default values.
        /// </summary>
        /// <value>
        ///     The list of default values.
        ///     An empty list is provided if no default values were specified.
        /// </value>
        public List<T> DefaultValues { get; }

        /// <summary>
        ///     Gets the setting name.
        /// </summary>
        /// <value>
        ///     The setting name.
        /// </value>
        public string Name { get; }

        /// <summary>
        ///     Gets the service resolver used to get the setting service.
        /// </summary>
        /// <value>
        ///     The service resolver used to get the setting service or null to provide the service directly to the accessor functions or to use <see cref="ServiceLocator" />
        /// </value>
        public Func<ISettingService> ServiceResolver { get; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Gets the setting value.
        /// </summary>
        /// <returns>
        ///     The setting value.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Before the setting value is retrieved, the setting is initialized with the default values if the value does not yet exist.
        ///     </note>
        /// </remarks>
        /// <exception cref="InvalidOperationException"> No setting service could be resolved or is available respectively. </exception>
        public T GetValue () => this.GetValue(null);

        /// <summary>
        ///     Gets the setting value.
        /// </summary>
        /// <param name="service"> The setting service to use. Can be null to use <see cref="ServiceResolver" /> or <see cref="ServiceLocator" />. </param>
        /// <returns>
        ///     The setting value.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Before the setting value is retrieved, the setting is initialized with the default values if the value does not yet exist.
        ///     </note>
        /// </remarks>
        /// <exception cref="InvalidOperationException"> No setting service could be resolved or is available respectively. </exception>
        public T GetValue (ISettingService service)
        {
            service = this.ResolveService(service);
            if (service == null)
            {
                throw new InvalidOperationException("No settings service available.");
            }

            service.InitializeValues(this.Name, this.DefaultValues);
            return service.GetValue<T>(this.Name);
        }

        /// <summary>
        ///     Gets the setting values.
        /// </summary>
        /// <returns>
        ///     The setting values.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Before the setting values are retrieved, the setting is initialized with the default values if the values do not yet exist.
        ///     </note>
        /// </remarks>
        /// <exception cref="InvalidOperationException"> No setting service could be resolved or is available respectively. </exception>
        public List<T> GetValues () => this.GetValues(null);

        /// <summary>
        ///     Gets the setting values.
        /// </summary>
        /// <param name="service"> The setting service to use. Can be null to use <see cref="ServiceResolver" /> or <see cref="ServiceLocator" />. </param>
        /// <returns>
        ///     The setting values.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Before the setting values are retrieved, the setting is initialized with the default values if the values do not yet exist.
        ///     </note>
        /// </remarks>
        /// <exception cref="InvalidOperationException"> No setting service could be resolved or is available respectively. </exception>
        public List<T> GetValues (ISettingService service)
        {
            service = this.ResolveService(service);
            if (service == null)
            {
                throw new InvalidOperationException("No settings service available.");
            }

            service.InitializeValues(this.Name, this.DefaultValues);
            return service.GetValues<T>(this.Name);
        }

        /// <summary>
        ///     Initializes the setting with the default values if the setting does not yet exist.
        /// </summary>
        /// <exception cref="InvalidOperationException"> No setting service could be resolved or is available respectively. </exception>
        public void Initialize () => this.Initialize(null);

        /// <summary>
        ///     Initializes the setting with the default values if the setting does not yet exist.
        /// </summary>
        /// <param name="service"> The setting service to use. Can be null to use <see cref="ServiceResolver" /> or <see cref="ServiceLocator" />. </param>
        /// <exception cref="InvalidOperationException"> No setting service could be resolved or is available respectively. </exception>
        public void Initialize (ISettingService service)
        {
            service = this.ResolveService(service);
            if (service == null)
            {
                throw new InvalidOperationException("No settings service available.");
            }

            service.InitializeValues(this.Name, this.DefaultValues);
        }

        /// <summary>
        ///     Sets the setting value.
        /// </summary>
        /// <param name="value"> The setting value. </param>
        /// <exception cref="InvalidOperationException"> No setting service could be resolved or is available respectively. </exception>
        public void SetValue (T value) => this.SetValue(null, value);

        /// <summary>
        ///     Sets the setting value.
        /// </summary>
        /// <param name="service"> The setting service to use. Can be null to use <see cref="ServiceResolver" /> or <see cref="ServiceLocator" />. </param>
        /// <param name="value"> The setting value. </param>
        /// <exception cref="InvalidOperationException"> No setting service could be resolved or is available respectively. </exception>
        public void SetValue (ISettingService service, T value)
        {
            service = this.ResolveService(service);
            if (service == null)
            {
                throw new InvalidOperationException("No settings service available.");
            }

            service.SetValue(this.Name, value);
        }

        /// <summary>
        ///     Sets the setting values.
        /// </summary>
        /// <param name="values"> The setting values. </param>
        /// <exception cref="InvalidOperationException"> No setting service could be resolved or is available respectively. </exception>
        public void SetValues (params T[] values) => this.SetValues(null, values);

        /// <summary>
        ///     Sets the setting values.
        /// </summary>
        /// <param name="values"> The setting values. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="values" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException"> No setting service could be resolved or is available respectively. </exception>
        public void SetValues (IEnumerable<T> values) => this.SetValues(null, values);

        /// <summary>
        ///     Sets the setting values.
        /// </summary>
        /// <param name="service"> The setting service to use. Can be null to use <see cref="ServiceResolver" /> or <see cref="ServiceLocator" />. </param>
        /// <param name="values"> The setting values. </param>
        /// <exception cref="InvalidOperationException"> No setting service could be resolved or is available respectively. </exception>
        public void SetValues (ISettingService service, params T[] values) => this.SetValues(null, (IEnumerable<T>)values);

        /// <summary>
        ///     Sets the setting values.
        /// </summary>
        /// <param name="service"> The setting service to use. Can be null to use <see cref="ServiceResolver" /> or <see cref="ServiceLocator" />. </param>
        /// <param name="values"> The setting values. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="values" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException"> No setting service could be resolved or is available respectively. </exception>
        public void SetValues (ISettingService service, IEnumerable<T> values)
        {
            service = this.ResolveService(service);
            if (service == null)
            {
                throw new InvalidOperationException("No settings service available.");
            }

            service.SetValues(this.Name, values);
        }

        private ISettingService ResolveService (ISettingService service) => service ?? this.ServiceResolver?.Invoke() ?? ServiceLocator.GetInstance<ISettingService>();

        #endregion
    }
}
