using System;
using System.Windows;
using System.Windows.Markup;

using RI.Framework.ComponentModel;
using RI.Framework.Composition;
using RI.Framework.Mvvm.View;
using RI.Framework.Mvvm.ViewModel;
using RI.Framework.Utilities;




namespace RI.Framework.Windows.Wpf.Markup
{
	/// <summary>
	///     Implements a WPF XAML markup extension to obtain instances.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The <see cref="InstanceLocator" /> is used in XAML to get instances from <see cref="IDependencyResolver" /> and assign them to properties in XAML.
	///         For example, this can be used to retrieve and attach a view model to a <see cref="FrameworkElement.DataContext" /> in MVVM scenarios.
	///     </para>
	///     <para>
	///         The instance to obtain can be either specified by its name, using the <see cref="Name" /> property, or its type, using the <see cref="Type" /> property.
	///         The used <see cref="IDependencyResolver" /> can also explicitly defined through the <see cref="Resolver" /> property.
	///     </para>
	///     <para>
	///         The used <see cref="IDependencyResolver" /> is determined in the following order:
	///         If <see cref="Resolver" /> is not null, that instance is used.
	///         If <see cref="DefaultResolver" /> is not null, that instance is used.
	///         <see cref="ServiceLocator" /> is used if neither <see cref="Resolver" /> nor <see cref="DefaultResolver" /> is set.
	///     </para>
	/// </remarks>
	[MarkupExtensionReturnType(typeof(object))]
	public sealed class InstanceLocator : MarkupExtension
	{
		#region Static Properties/Indexer

		/// <summary>
		///     Gets or sets the default dependency resolver to use.
		/// </summary>
		/// <value>
		///     The default dependency resolver to use.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is null.
		///     </para>
		/// </remarks>
		public static IDependencyResolver DefaultResolver { get; set; }

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="InstanceLocator" />.
		/// </summary>
		public InstanceLocator ()
		{
			this.Resolver = null;
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
		public string Name { get; set; }

		/// <summary>
		///     Gets or sets the dependency resolver to use.
		/// </summary>
		/// <value>
		///     The dependency resolver to use.
		/// </value>
		public IDependencyResolver Resolver { get; set; }

		/// <summary>
		///     Gets or sets the type of the instance to obtain.
		/// </summary>
		/// <value>
		///     The type of the instance to obtain.
		/// </value>
		public Type Type { get; set; }

		private IDependencyResolver UsedResolver => this.Resolver ?? InstanceLocator.DefaultResolver ?? ServiceLocator.Resolver;

		#endregion




		#region Instance Methods

		private object GetValue (string name)
		{
			if (name.IsNullOrEmptyOrWhitespace())
			{
				return null;
			}

			return this.UsedResolver.GetInstance(name);
		}

		private object GetValue (Type type)
		{
			if (type == null)
			{
				return null;
			}

			return this.UsedResolver.GetInstance(type);
		}

		private void ProcessValue (object value)
		{
			if (value == null)
			{
				return;
			}

			CompositionContainer container = this.UsedResolver.GetInstance<CompositionContainer>();

			if (container != null)
			{
				container.ResolveImports(value, CompositionFlags.Normal);
			}

			if (value is IViewModel)
			{
				IViewModel viewModel = (IViewModel)value;
				if (!viewModel.IsInitialized)
				{
					viewModel.Initialize();
				}
			}

			if (value is IView)
			{
				IView view = (IView)value;
				if (!view.IsInitialized)
				{
					view.Initialize();
				}
			}

			if (value is FrameworkElement)
			{
				FrameworkElement frameworkElement = (FrameworkElement)value;
				if (frameworkElement.DataContext != null)
				{
					if (container != null)
					{
						container.ResolveImports(frameworkElement.DataContext, CompositionFlags.Normal);
					}

					if (frameworkElement.DataContext is IViewModel)
					{
						IViewModel viewModel = (IViewModel)frameworkElement.DataContext;
						if (!viewModel.IsInitialized)
						{
							viewModel.Initialize();
						}
					}

					if (frameworkElement.DataContext is IView)
					{
						IView view = (IView)frameworkElement.DataContext;
						if (!view.IsInitialized)
						{
							view.Initialize();
						}
					}
				}
			}
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override object ProvideValue (IServiceProvider serviceProvider)
		{
			IProvideValueTarget targetProvider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (targetProvider != null)
			{
				object target = targetProvider.TargetObject;
				this.ProcessValue(target);
			}

			object value = this.GetValue(this.Name) ?? this.GetValue(this.Type);
			this.ProcessValue(value);

			return value;
		}

		#endregion
	}
}
