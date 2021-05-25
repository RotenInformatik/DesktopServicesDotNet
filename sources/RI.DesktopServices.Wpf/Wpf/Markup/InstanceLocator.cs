using System;
using System.Windows;
using System.Windows.Markup;

using RI.DesktopServices.UiContainer;




namespace RI.DesktopServices.Wpf.Markup
{
    /// <summary>
    ///     Implements a WPF XAML markup extension to obtain instances and assigns them to data contexts.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The <see cref="InstanceLocator" /> is used in XAML to get instances from either an
    ///         <see cref="IServiceProvider" /> (<see cref="ServiceProvider" />) or a resolver function (
    ///         <see cref="Resolver" />) and
    ///         assign them to properties in XAML.
    ///         For example, this can be used to retrieve and attach a view model to a
    ///         <see cref="FrameworkElement.DataContext" /> in MVVM scenarios.
    ///     </para>
    ///     <para>
    ///         The instance to obtain can be either specified by its name, using the <see cref="Name" /> property, or its
    ///         type, using the <see cref="Type" /> property. <see cref="Name" /> is treated as the string representation of
    ///         the <see cref="Type" /> to use.
    ///     </para>
    ///     <para>
    ///         Instance resolving is done in the following order:
    ///         If <see cref="Resolver" /> is not null, that function is used.
    ///         If <see cref="Resolver" /> is null or returns null, <see cref="ServiceProvider" /> is used.
    ///         set.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    [MarkupExtensionReturnType(typeof(object))]
    public sealed class InstanceLocator : MarkupExtension
    {
        #region Static Properties/Indexer

        /// <summary>
        ///     Gets or sets the resolver function to use.
        /// </summary>
        /// <value>
        ///     The resolver function to use.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is null.
        ///     </para>
        /// </remarks>
        public static Func<Type, object> Resolver { get; set; }

        /// <summary>
        ///     Gets or sets the service provider to use.
        /// </summary>
        /// <value>
        ///     The service provider to use.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is null.
        ///     </para>
        /// </remarks>
        public static IServiceProvider ServiceProvider { get; set; }

        #endregion




        #region Static Methods

        internal static IRegionService GetInstanceForRegionBinder () =>
            (InstanceLocator.Resolver?.Invoke(typeof(IRegionService)) ??
             InstanceLocator.ServiceProvider.GetService(typeof(IRegionService))) as IRegionService;

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="InstanceLocator" />.
        /// </summary>
        public InstanceLocator ()
        {
            this.Name = null;
            this.Type = null;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets or sets the name of the instance to obtain.
        /// </summary>
        /// <value>
        ///     The name of the instance to obtain.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is null.
        ///     </para>
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the type of the instance to obtain.
        /// </summary>
        /// <value>
        ///     The type of the instance to obtain.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is null.
        ///     </para>
        /// </remarks>
        public Type Type { get; set; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Gets the instance of a specified type.
        /// </summary>
        /// <param name="type"> The type an instance is to used. </param>
        /// <returns>
        ///     The instance or null if no instance of that type is available.
        /// </returns>
        public object GetInstance (Type type)
        {
            if (type == null)
            {
                return null;
            }

            return InstanceLocator.Resolver?.Invoke(type) ?? InstanceLocator.ServiceProvider.GetService(type);
        }

        /// <summary>
        ///     Gets the instance of a specified type.
        /// </summary>
        /// <typeparam name="T"> The type an instance is to used. </typeparam>
        /// <returns>
        ///     The instance or null if no instance of that type is available.
        /// </returns>
        public T GetInstance <T> ()
            where T : class =>
            this.GetInstance(typeof(T)) as T;

        /// <summary>
        ///     Gets the instance of a specified type (as its string representation).
        /// </summary>
        /// <param name="name"> The type name an instance is to used. </param>
        /// <returns>
        ///     The instance or null if no instance of that type is available.
        /// </returns>
        public object GetInstance (string name)
        {
            if (name == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The string is empty.", nameof(name));
            }

            return this.GetInstance(this.ResolveTypeFromName(name));
        }

        /// <summary>
        ///     Gets the instance of a specified type (as its string representation).
        /// </summary>
        /// <typeparam name="T"> The type an instance is to used. </typeparam>
        /// <param name="name"> The type name an instance is to used. </param>
        /// <returns>
        ///     The instance or null if no instance of that type is available.
        /// </returns>
        public T GetInstance <T> (string name)
            where T : class =>
            this.GetInstance(name) as T;

        private Type ResolveTypeFromName (string name)
        {
            if (name == null)
            {
                return null;
            }

            return Type.GetType(name, false, true);
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override object ProvideValue (IServiceProvider serviceProvider)
        {
            serviceProvider.GetService(typeof(IProvideValueTarget));

            return this.GetInstance(this.Name) ?? this.GetInstance(this.Type);
        }

        #endregion
    }
}
